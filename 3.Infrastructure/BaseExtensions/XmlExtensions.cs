using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using Infrastructure.Phrases;

namespace Infrastructure.BaseExtensions;

/// <summary>
/// Методы-расширения для <see cref="XmlNode" />.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class XmlExtensions
{
    /// <summary>
    /// Поиск и получение узла-потомка по составному ключу.
    /// </summary>
    /// <remarks>
    /// Родители и дети разделяются в составном ключе символом ":".
    /// </remarks>
    /// <param name="xmlNode">Узел, от которого производится поиск.</param>
    /// <param name="compositeKey">Составной ключ.</param>
    /// <param name="exception">Исключение, если оно имело место быть.</param>
    /// <returns>Найденный узел-потомок.</returns>
    public static XmlNode GetXmlSubNode(this XmlNode xmlNode, string compositeKey, out Exception exception)
    {
        exception = null;
        
        // Разделяем составной ключ (строку) на составляющие в виде списка
        var list = compositeKey.Split(':').ToList();
        if (list.Contains(string.Empty))
        {
            // Неправильный ключ
            exception = new XmlException(CommonPhrases.Exception_InvalidKeyName);
            return null;
        }
        
        try
        {
            var node = xmlNode;
            foreach (var item in list)
            {
                node = node?.Cast<XmlNode>().LastOrDefault(n => n.Name == item);
            }

            if (node is null)
                exception = new XmlException(CommonPhrases.Exception_NodeNotFound);
            return node;
        }
        catch (Exception ex)
        {
            exception = ex;
            return null;
        }    
    }
    
    /// <inheritdoc cref="GetXmlSubNode"/>
    /// <summary>
    /// Поиск и получение списка узлов-потомков по составному ключу.
    /// </summary>
    /// <returns>Список найденных узлов-потомков.</returns>
    [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
    public static List<XmlNode> GetXmlSubNodeList(this XmlNode xmlNode, string compositeKey, out Exception exception)
    {
        exception = null;
        var xmlNodeList = new List<XmlNode>();

        // Разделяем составной ключ (строку) на составляющие в виде списка
        var list = compositeKey.Split(':').ToList();
        if (list.Contains(string.Empty))
        {
            // Неправильный ключ
            exception = new XmlException(CommonPhrases.Exception_InvalidKeyName);
            return null;
        }
        
        try
        {
            var node = xmlNode;
            foreach (var item in list)
            {
                xmlNodeList = node?.Cast<XmlNode>().Where(n => n.Name == item).ToList();
                node = xmlNodeList?.Last();
            }
            return xmlNodeList;
        }
        catch (Exception ex)
        {
            exception = ex;
            return null;
        }    
    }
    
    /// <inheritdoc cref="GetXmlSubNode"/>
    /// <summary>
    /// Создание узла-потомка по составному ключу.
    /// </summary>
    /// <returns>Созданный XML-узел.</returns>
    public static XmlNode CreateXmlSubNode(this XmlNode xmlNode, string compositeKey, out Exception exception)
    {
        exception = null;
        
        // Разделяем составной ключ (строку) на составляющие в виде списка
        var list = compositeKey.Split(':').ToList();
        if (list.Contains(string.Empty))
        {
            // Неправильный ключ
            exception = new XmlException(CommonPhrases.Exception_InvalidKeyName);
            return null;
        }
        
        try
        {
            var node = xmlNode;
            var docXml = xmlNode.NodeType == XmlNodeType.Document 
                ? (XmlDocument)xmlNode
                : xmlNode.OwnerDocument;
            foreach (var item in list)
            {
                var tmpNode = node?.Cast<XmlNode>().LastOrDefault(n => n.Name == item);
                if (tmpNode is null)
                {
                    var elem = docXml!.CreateElement(item);
                    node!.AppendChild(elem);
                    node = elem;
                }
                else
                {
                    node = tmpNode;
                }
            }
            // var txtNode = docXml!.CreateTextNode(null);
            // node!.AppendChild(txtNode);
            return node;
        }
        catch (Exception ex)
        {
            exception = ex;
            return null;
        }    
    }  
}