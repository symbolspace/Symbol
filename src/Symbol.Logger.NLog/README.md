# Infrastructure Symbol.Logger.NLog
> 基于 NLog。

支持运行时：
* .NET Framework 3.5
* .NET Framework 4.0
* .NET Framework 4.5
* .NET Framework 4.6.2
* .NET Framework 4.8
* .NET Standard 2.0
* .NET 6.0
* .NET 7.0
* .NET 8.0

## 调用代码
```csharp
using Symbol.Logger;

class Program {
    static void Main(){
        //设置全局日志提供者。
        LoggerProvider.SetProvider(ConfigurationManager.AppSettings["LoggerProvider"]);
	}
}
```
## App.config
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <sectionGroup name="userSettings" type="System.Configuration.UserSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" >
    </sectionGroup>
  </configSections>
  <appSettings>
    <add key="LoggerProvider" value="NLog" />
  </appSettings>
</configuration>
```

## NLog.config
> 最终级项目中，包含一个名为“NLog.config”的配置文件。

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="Off" internalLogFile="${specialfolder:folder=MyDocuments:cached=true}/${companyName}/${appName}/${dirName}/nlog-internal.log">
	<variable name="companyName" value="YourCompanyName"/>
	<variable name="appName" value="YourAppName"/>
	<variable name="dirName" value="Logs"/>
	<targets>
		<target name="logfileObservableObject"
				xsi:type="File"
				fileName="${specialfolder:folder=MyDocuments:cached=true}/${companyName}/${appName}/${dirName}/observableObject-info-${shortdate}-current.log"
				layout="${longdate} [${uppercase:${level}}] ${message} ${exception:format=ToString}"
				maxArchiveFiles="30"
				archiveFileName="${specialfolder:folder=MyDocuments:cached=true}/${companyName}/${appName}/${dirName}/observableObject-info-${shortdate}-archive${###}.log"
				createDirs="true"
				archiveAboveSize="102400"
				archiveEvery="Day"
				encoding="UTF-8"
		/>
		<target name="logfileObservableObjectDebug"
				xsi:type="File"
				fileName="${specialfolder:folder=MyDocuments:cached=true}/${companyName}/${appName}/${dirName}/observableObject-debug-${shortdate}-current.log"
				layout="${longdate} [${uppercase:${level}}] ${message} ${exception:format=ToString}"
				maxArchiveFiles="30"
				archiveFileName="${specialfolder:folder=MyDocuments:cached=true}/${companyName}/${appName}/${dirName}/observableObject-debug-${shortdate}-archive${###}.log"
				createDirs="true"
				archiveAboveSize="204800"
				archiveEvery="Day"
				encoding="UTF-8"
		/>
		<target name="logfileOther"
				xsi:type="File"
				fileName="${specialfolder:folder=MyDocuments:cached=true}/${companyName}/${appName}/${dirName}/other-${shortdate}-current.log"
				layout="${longdate} [${uppercase:${level}}] ${logger} ${message} ${exception:format=ToString}"
				maxArchiveFiles="30"
				archiveFileName="${specialfolder:folder=MyDocuments:cached=true}/${companyName}/${appName}/${dirName}/other-${shortdate}-archive${###}.log"
				createDirs="true"
				archiveAboveSize="102400"
				archiveEvery="Day"
				encoding="UTF-8"
		/>
		<target name="logconsole" xsi:type="Console"
				layout="${longdate} [${uppercase:${level}}] ${callsite}(${callsite-filename:includeSourcePath=False}:${callsite-linenumber}) - ${message} ${exception:format=ToString}"
		/>
	</targets>
	<rules>
		<!-- final为true时，表示后面的规则不生效  -->
		<logger name="Symbol.Logger.DebugTime"  writeTo="logfileObservableObjectDebug" />
		<logger name="LoggerObservableObject.*" levels="Trace,Debug" writeTo="logfileObservableObjectDebug" />
		<logger name="LoggerObservableObject.*" levels="Info,Error,Warn,Fatal" writeTo="logfileObservableObject"/>
		<!--<logger name="*" writeTo="logfileOther" />-->
	</rules>
</nlog>
```

模板说明：
* **YourCompanyName**：公司名称。
`<variable name="companyName" value="YourCompanyName"/>`
* **YourAppName**：产品名称。
`<variable name="appName" value="YourAppName"/>`
* **保存位置**：保存到“我的文档”目录。
`${specialfolder:folder=MyDocuments:cached=true}`