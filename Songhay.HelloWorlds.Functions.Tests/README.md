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
