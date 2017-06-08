# VkNewsTracker
Track your vk.com newsfeed updates with regex filtering and telegram notifications

# Configuration
## appsettings.json:
```json
{
   "ApplicationId":5891549,
   "AccessToken":"YOUR_TOKEN",
   "AccessTokenLifetime":3600,
   "UserId":"YOUR_USER_ID",
   "RefreshPeriod":3600,
   "LastUpdate":"2017-01-08T05:38:19.4728478Z",
   "IncludedPatterns":[
      ".*London.*",
      ".*Москва.*"
   ],
   "ExcludedPatterns":[

   ]
}
```
## NLog.config
```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="Trace" internalLogFile="c:\temp\nlog-internal.log">

  <extensions>
    <add assembly="VkNewsTracker.NLog.Telegram" />
  </extensions>

  <targets async="true">
    <target name="logfile" xsi:type="File"
            layout="${longdate} | ${level:uppercase=true} | ${logger} | ${message} | ${exception:format=tostring}"
            fileName="${basedir}/logs/Log.current.txt"
            archiveFileName="${basedir}/logs/archives/log.${shortdate}.{#}.txt"
            archiveEvery="Day"
            archiveNumbering = "Rolling"
            maxArchiveFiles="10"
            concurrentWrites="true"
            keepFileOpen="false" />

    <target xsi:type="Telegram"
            name ="telegram"
            layout="${message}"
            botToken ="XXX"
            chatId="-XXX for chats or XXX for users" />
  </targets>

  <rules>
    <logger name="*" minlevel="Trace" writeTo="logfile" />
    <logger name="*" minlevel="Debug" writeTo="telegram" />
  </rules>
</nlog>
```

# Depedencies
* .NET Core 1.1
* https://github.com/narfunikita/NLog.Telegram - modified for compatibility with .NET Core
* https://github.com/khrabrovart/Citrina - modified for some additional fields support
