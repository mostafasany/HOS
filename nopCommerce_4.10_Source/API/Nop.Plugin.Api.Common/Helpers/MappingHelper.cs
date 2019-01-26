using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Common.Factories;

namespace Nop.Plugin.Api.Common.Helpers
{
    // TODO: Think of moving the mapping helper in the delta folder
    public class MappingHelper : IMappingHelper
    {
        public void Merge(object source, object destination)
        {
            Dictionary<string, object> sourcePropertyValuePairs = source.GetType()
                .GetProperties()
                .ToDictionary(property => property.Name, property => property.GetValue(source));

            SetValues(sourcePropertyValuePairs, destination, destination.GetType(), null);
        }

        public void SetValues(Dictionary<string, object> propertyNameValuePairs, object objectToBeUpdated,
            Type propertyType, Dictionary<object, object> objectPropertyNameValuePairs, bool handleComplexTypeCollections = false)
        {
            objectPropertyNameValuePairs?.Add(objectToBeUpdated, propertyNameValuePairs);

            foreach (KeyValuePair<string, object> propertyNameValuePair in propertyNameValuePairs) SetValue(objectToBeUpdated, propertyNameValuePair, objectPropertyNameValuePairs, handleComplexTypeCollections);
        }

        private void AddBaseItemInCollection(object newItem, IList collection, Type collectionElementsType)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(collectionElementsType);

            string newItemValueToString = newItem.ToString();

            if (converter.IsValid(newItemValueToString)) collection.Add(converter.ConvertFrom(newItemValueToString));
        }

        private void AddNewItemInCollection(Dictionary<string, object> newProperties, IList collection, Type collectionElementsType, Dictionary<object, object> objectPropertyNameValuePairs, bool handleComplexTypeCollections)
        {
            object newInstance = Activator.CreateInstance(collectionElementsType);

            PropertyInfo[] properties = collectionElementsType.GetProperties();

            SetEveryDatePropertyThatIsNotSetToDateTimeUtcNow(newProperties, properties);

            SetValues(newProperties, newInstance, collectionElementsType, objectPropertyNameValuePairs, handleComplexTypeCollections);

            collection.Add(newInstance);
        }

        private void AddOrUpdateComplexItemInCollection(Dictionary<string, object> newProperties, IList collection, Type collectionElementsType,
            Dictionary<object, object> objectPropertyNameValuePairs, bool handleComplexTypeCollections)
        {
            if (newProperties.ContainsKey("Id"))
            {
                // Every element in collection, that is not System type should have an id.
                int id = int.Parse(newProperties["Id"].ToString());

                object itemToBeUpdated = null;

                // Check if there is already an item with this id in the collection.
                foreach (object item in collection)
                    if (int.Parse(item.GetType()
                            .GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                            .GetValue(item)
                            .ToString()) == id)
                    {
                        itemToBeUpdated = item;
                        break;
                    }

                if (itemToBeUpdated == null)
                    AddNewItemInCollection(newProperties, collection, collectionElementsType, objectPropertyNameValuePairs, handleComplexTypeCollections);
                else
                    SetValues(newProperties, itemToBeUpdated, collectionElementsType, objectPropertyNameValuePairs, handleComplexTypeCollections);
            }
            // It is a new item.
            else
            {
                AddNewItemInCollection(newProperties, collection, collectionElementsType, objectPropertyNameValuePairs, handleComplexTypeCollections);
            }
        }

        // Used in the SetValue private method and also in the Delta.
        private void ConvertAndSetValueIfValid(object objectToBeUpdated, PropertyInfo objectProperty, object propertyValue)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(objectProperty.PropertyType);

            string propertyValueAsString = string.Format(CultureInfo.InvariantCulture, "{0}", propertyValue);

            if (converter.IsValid(propertyValueAsString))
            {
                object convertedValue = converter.ConvertFromInvariantString(propertyValueAsString);

                objectProperty.SetValue(objectToBeUpdated, convertedValue);
            }
        }

        private IList CreateEmptyList(Type listItemType)
        {
            Type listType = typeof(List<>);
            Type constructedListType = listType.MakeGenericType(listItemType);
            object list = Activator.CreateInstance(constructedListType);

            return list as IList;
        }

        // We need this method, because the default value of DateTime is not in the sql server DateTime range and we will get an exception if we use it.
        private void SetEveryDatePropertyThatIsNotSetToDateTimeUtcNow(Dictionary<string, object> newProperties, PropertyInfo[] properties)
        {
            foreach (PropertyInfo property in properties)
                if (property.PropertyType == typeof(DateTime))
                {
                    var keyFound = false;

                    // We need to loop through the keys, because the key may contain underscores in its name, which won't match the jsonProperty name.
                    foreach (string key in newProperties.Keys)
                        if (key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            keyFound = true;
                            break;
                        }

                    if (!keyFound) newProperties.Add(property.Name, DateTime.UtcNow);
                }
        }

        private void SetValue(object objectToBeUpdated, KeyValuePair<string, object> propertyNameValuePair, Dictionary<object, object> objectPropertyNameValuePairs, bool handleComplexTypeCollections)
        {
            string propertyName = propertyNameValuePair.Key;
            object propertyValue = propertyNameValuePair.Value;

            PropertyInfo propertyToUpdate = objectToBeUpdated.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propertyToUpdate != null)
            {
                // This case handles nested properties.
                if (propertyValue != null && propertyValue is Dictionary<string, object>)
                {
                    object valueToUpdate = propertyToUpdate.GetValue(objectToBeUpdated);

                    if (valueToUpdate == null)
                    {
                        // Check if there is registered factory for this type.
                        Type factoryType = typeof(IFactory<>);
                        Type factoryTypeForCurrentProperty = factoryType.MakeGenericType(propertyToUpdate.PropertyType);
                        object initializerFactory = ((NopEngine) EngineContext.Current).ServiceProvider.GetService(factoryTypeForCurrentProperty);

                        if (initializerFactory != null)
                        {
                            MethodInfo initializeMethod = factoryTypeForCurrentProperty.GetMethod("Initialize");

                            valueToUpdate = initializeMethod.Invoke(initializerFactory, null);
                        }
                        else
                        {
                            valueToUpdate = Activator.CreateInstance(propertyToUpdate.PropertyType);
                        }

                        propertyToUpdate.SetValue(objectToBeUpdated, valueToUpdate);
                    }

                    // We need to use GetValue method to get the actual instance of the jsonProperty. objectProperty is the jsonProperty info.
                    SetValues((Dictionary<string, object>) propertyValue, valueToUpdate,
                        propertyToUpdate.PropertyType, objectPropertyNameValuePairs);
                    // We expect the nested properties to be classes which are refrence types.
                    return;
                }
                // This case hadles collections.

                if (propertyValue != null && propertyValue is ICollection<object>)
                {
                    var propertyValueAsCollection = propertyValue as ICollection<object>;

                    Type collectionElementsType = propertyToUpdate.PropertyType.GetGenericArguments()[0];
                    object collection = propertyToUpdate.GetValue(objectToBeUpdated);

                    if (collection == null)
                    {
                        collection = CreateEmptyList(collectionElementsType);
                        propertyToUpdate.SetValue(objectToBeUpdated, collection);
                    }

                    //this is a hack to fix a bug when "collection" cannot be cast to IList (ex:  it's a HashSet for Order.OrderItems)
                    var collectionAsList = collection as IList;
                    if (collectionAsList == null)
                    {
                        collectionAsList = CreateEmptyList(collectionElementsType);

                        var collectionAsEnumerable = collection as IEnumerable;
                        foreach (object collectionItem in collectionAsEnumerable) collectionAsList.Add(collectionItem);

                        collection = collectionAsList;
                        propertyToUpdate.SetValue(objectToBeUpdated, collection);
                    }

                    foreach (object item in propertyValueAsCollection)
                        if (collectionElementsType.Namespace != "System")
                        {
                            if (handleComplexTypeCollections)
                                AddOrUpdateComplexItemInCollection(item as Dictionary<string, object>,
                                    collection as IList,
                                    collectionElementsType, objectPropertyNameValuePairs, handleComplexTypeCollections);
                        }
                        else
                        {
                            AddBaseItemInCollection(item, collection as IList, collectionElementsType);
                        }

                    return;
                }

                // This is where the new value is beeing set to the object jsonProperty using the SetValue function part of System.Reflection.
                if (propertyValue == null)
                    propertyToUpdate.SetValue(objectToBeUpdated, null);
                else if (propertyValue is IConvertible)
                    ConvertAndSetValueIfValid(objectToBeUpdated, propertyToUpdate, propertyValue);
                else
                    propertyToUpdate.SetValue(objectToBeUpdated, propertyValue);
            }
        }
    }
}