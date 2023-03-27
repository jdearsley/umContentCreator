﻿using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Serialization;
using Umbraco.Extensions;
using umContentCreator.Core.Interfaces;
using umContentCreator.Core.PropertyEditors;

namespace umContentCreator.Core.Services;

public class UmContentCreatorInjectorService : IUmContentCreatorInjectorService
{
    private readonly IDataTypeService _dataTypeService;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IDataValueEditorFactory _editorFactory;
    private bool _contentModified = false;

    public UmContentCreatorInjectorService(IDataTypeService dataTypeService, IShortStringHelper shortStringHelper, IDataValueEditorFactory editorFactory)
    {
        _dataTypeService = dataTypeService;
        _shortStringHelper = shortStringHelper;
        _editorFactory = editorFactory;
    }

    public void AddUmContentCreatorToExistingContentTypes(IEnumerable<IContentType> contentTypes)
    {
        const string propertyEditorAlias = "umContentCreator";
        const string propertyName = "Content Creator";
        const string propertyTabName = "Content Creator";

        var dataType = GetDataType(propertyEditorAlias);

        foreach (var contentType in contentTypes)
        {
            if (contentType.PropertyTypeExists(propertyEditorAlias))
            {
                continue;
            }

            var contentCreatorPropertyGroup = GetContentCreatorPropertyGroup(contentType, propertyTabName);
            var contentCreatorPropertyType = new PropertyType(_shortStringHelper, dataType, propertyEditorAlias)
            {
                Name = propertyName,
                Description = "Automatically added Content Creator property",
                Mandatory = false
            };

            contentCreatorPropertyGroup.PropertyTypes?.Add(contentCreatorPropertyType);
            _contentModified = true;
        }
    }

    public bool GetContentModificationStatus()
    {
        return _contentModified;
    }
    
    private IDataType GetDataType(string propertyEditorAlias)
    {
        var dataType = _dataTypeService.GetByEditorAlias(propertyEditorAlias).FirstOrDefault();
        if (dataType != null)
        {
            return dataType;
        }
        
        var editor = new UmContentCreator(_editorFactory);
        dataType = new DataType(editor, new ConfigurationEditorJsonSerializer())
        {
            Name = "umContentCreator"
        };
        _dataTypeService.Save(dataType);

        return dataType;
    }
    
    private PropertyGroup GetContentCreatorPropertyGroup(IContentTypeBase contentType, string propertyTabName)
    {
        var contentCreatorPropertyGroup = contentType.PropertyGroups.FirstOrDefault(x => x.Name == propertyTabName);

        if (contentCreatorPropertyGroup != null)
        {
            return contentCreatorPropertyGroup;
        }
        
        contentCreatorPropertyGroup = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Name = propertyTabName,
            Alias = propertyTabName.ToSafeAlias(_shortStringHelper)
        };
        contentType.PropertyGroups.Add(contentCreatorPropertyGroup);

        return contentCreatorPropertyGroup;
    }
}