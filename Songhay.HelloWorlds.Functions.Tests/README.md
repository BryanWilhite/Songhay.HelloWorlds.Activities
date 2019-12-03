# Testing with Azure Functions Core Tools

Most of the tests in this project depend on [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local).

## installing Azure Functions Core Tools

The documented installation command is:

```console
npm install -g azure-functions-core-tools
```

But, in case you run into an error like this:

```console
attempting to GET "https://functionscdn.azureedge.net/public/2.7.1948/Azure.Functions.Cli.linux-x64.2.7.1948.zip"
[------------------] Downloading Azure Functions Core Toolsevents.js:174
      throw er; // Unhandled 'error' event
      ^

Error: EACCES: permission denied, mkdir '/usr/lib/node_modules/azure-functions-core-tools/bin'
Emitted 'error' event at:
    at errorOrDestroy (internal/streams/destroy.js:107:12)
    at DuplexWrapper.onerror (_stream_readable.js:732:7)
    at DuplexWrapper.emit (events.js:198:13)
    at Readable.<anonymous> (/usr/lib/node_modules/azure-functions-core-tools/node_modules/duplexer2/index.js:49:12)
    at Readable.emit (events.js:198:13)
    at Writable.emit (events.js:203:15)
    at errorOrDestroy (internal/streams/destroy.js:107:12)
    at onwriteError (_stream_writable.js:436:5)
    at onwrite (_stream_writable.js:461:5)
    at ProxyWriter.emit (events.js:203:15)
    at FileWriter.emit (events.js:198:13)
    at FileWriter.Abstract.error (/usr/lib/node_modules/azure-functions-core-tools/node_modules/fstream/lib/abstract.js:63:13)
    at /usr/lib/node_modules/azure-functions-core-tools/node_modules/fstream/lib/writer.js:171:25
    at /usr/lib/node_modules/azure-functions-core-tools/node_modules/mkdirp/index.js:47:53
    at FSReqWrap.oncomplete (fs.js:153:21)
npm ERR! code ELIFECYCLE
npm ERR! errno 1
npm ERR! azure-functions-core-tools@2.7.1948 postinstall: `node lib/install.js`
npm ERR! Exit status 1
npm ERR! 
npm ERR! Failed at the azure-functions-core-tools@2.7.1948 postinstall script.
npm ERR! This is probably not a problem with npm. There is likely additional logging output above.
```

…an alternative command might be useful:

```console
sudo npm install -g azure-functions-core-tools --unsafe-perm=true --allow-root
```

📚Reference: <https://github.com/Azure/azure-functions-core-tools/issues/331>

This issue might be exclusive to a Linux environment.

## running the tests

### from Visual Studio

In Visual Studio, ensure that the `Songhay.HelloWorlds.Functions` project is the [StartUp project](https://blogs.msdn.microsoft.com/zainnab/2010/05/09/choosing-the-startup-project/). Hitting `F5` will start the local version of Azure Functions but it will _not_ be possible to debug tests while Azure Functions is running. Visual Studio Code (VSCode) can be handy here as the tests, specifically the `HttpTriggerTests` [tests](./HttpTriggerTests.cs), can be run from VSCode (and any breakpoints set in Functions and/or Activities code should be hit).

### from Visual Studio Code

In Visual Studio Code this task is defined:

```json
{
    "type": "func",
    "dependsOn": "build",
    "options": {
        "cwd": "${workspaceFolder}/Songhay.HelloWorlds.Functions"
    },
    "command": "start --build --csharp",
    "isBackground": true,
    "problemMatcher": "$func-watch"
}
```

It is based on [a command from the docs](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#c).

It is also not possible to debug tests from VSCode while this IDE is running a local version of Azure Functions and Visual Studio would come in handy to debug tests (⚠ the HTTP calls in test code should hit debug breakpoints but the decoupled function logic should not hit). However it _is_ possible to run tests from VSCode _without_ debugging them.

## related links

- “[Optimize the performance and reliability of Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-best-practices)”
- “[Create your first function using Visual Studio Code](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-function-vs-code)” [⚠ warning: see [#7](https://github.com/BryanWilhite/Songhay.HelloWorlds.Activities/issues/7)]
- “[Introducing Azure Functions 2.0](https://azure.microsoft.com/en-us/blog/introducing-azure-functions-2-0/)”
- [Azure serverless community library](https://www.serverlesslibrary.net/)
- [Quickly Restore your Local Settings File for Azure Functions](https://microsoft.github.io/AzureTipsAndTricks/blog/tip136.html)

@[BryanWilhite](https://twitter.com/BryanWilhite)
