# HomeMatic-XmlApi-Lib
Library that wraps the HomeMatic XML-API Addon in C# for use in .NET projects

The XML-API Addon (http://www.homematic-inside.de/software/xml-api) is a free collection of scripts, packaged as installable addon for the 'HomeMatic' home automation system. Once installed to a CCU, the package provides a very simple but broad and powerful API as an alternative for the included XML-RPC interface.

The HomeMatic-XmlApi-Lib is intended to close the gap for developers using a .NET environment for their projects.

## Contents

### LIB_HomeMaticXmlApi

The library itself. Using the *HMApiWrapper* you can connect to a HomeMatic CCU and obtain devices with current states. The classes *HMDevice*, *HMDeviceChannel*, *HMDeviceDataPoint* and *HMSystemVariable* are merely here for persistence of received data.

### TST_HomeMaticXmlApi

A simple Windows Forms application that'll use the library to output all connected devices in a treeview.

## Current Version

The current version of HomeMatic-XmlApi-Lib is a very plain and early alpha release. I just implemented the most necessary functions the get hold of devices and its statuses. 
