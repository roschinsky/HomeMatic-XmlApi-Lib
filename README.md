# HomeMatic-XmlApi-Lib
The library wraps the HomeMatic XML-API Addon in C# for use in .NET projects

The XML-API Addon (http://www.homematic-inside.de/software/xml-api) is a free collection of scripts, packaged as installable addon for the 'HomeMatic' home automation system. Once installed to a CCU, the package provides a very simple but broad and powerful API as an alternative for the included XML-RPC interface.

The HomeMatic-XmlApi-Lib is intended to workaround the known issues with the native XML-RPC API provided by Homematic CCU2 and to close the gap for developers using a .NET environment for their projects.

## Contents

### LIB_HomeMaticXmlApi

The library itself. Using the *HMApiWrapper* you can connect to a HomeMatic CCU and obtain devices with current states. The classes *HMDevice*, *HMDeviceChannel*, *HMDeviceDataPoint* and *HMSystemVariable* are merely here for persistence of received data.

![Class diagramm](https://troschinsky.files.wordpress.com/2015/12/homematicxmlapi_lib.png?w=600)

### TST_HomeMaticXmlApi

A simple Windows Forms application that'll use the library to output all connected devices in a treeview. You can use the methods of the wrapper class to refresh/update the states of all devices. If there are any "Fast Update Devices" defined (select in treeview and click "Add" on the right hand side) you can use the radio button to just refresh some states in devices list and gain a significant performance improvement.

![Tester Windows Forms application](https://troschinsky.files.wordpress.com/2015/12/homematicxmlapi_tst.png)

You can also browse the internal data structure of Homematic devices, channels and its data points. Channels and data points can be set by double-clicking the item in the treeview. You'll need to enter values to set as plain text, like 'true' or '21.5' This will work with variables as well, so you're able to set the values of variables by using this library.

## Current Version

The current version of HomeMatic-XmlApi-Lib is a plain and early beta release. Most of the functions provided by the XML-API Add-On are supported.
