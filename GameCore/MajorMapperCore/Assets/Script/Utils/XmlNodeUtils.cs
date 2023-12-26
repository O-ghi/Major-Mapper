using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XmlNodeUtils
{
    public static XmlAttribute GetIndexXmlAttributeCollection(XmlAttributeCollection xml, int i)
    {
        return xml[i];
    }

    public static XmlNode GetIndexXmlNodeList(XmlNodeList xml, int i)
    {
        return xml[i];
    }
    public static int GetCountXmlNamedNodeMap(XmlAttributeCollection xml)
    {
        return xml.Count;
    }
    public static int GetCountXmlNodeList(XmlNodeList xml)
    {
        return xml.Count;
    }

}
