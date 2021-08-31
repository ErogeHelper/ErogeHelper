[简体中文](https://github.com/luojunyuan/Eroge-Helper/blob/master/README_zh-cn.md)

### How to build

Use VS2019 or Rider to clone `https://github.com/Eroge-Helper/ErogeHelper` down.

Fill the command parameters in ErogeHelper's properties like `"D:\Ra-se-n\C' - can't live without you\c.exe" /le` and press F5 to run.

one is full path of game, '/le' or '-le' to start with Locate Emulator

`Ctrl+Shift+B` to compile all things, then you can check other parts.

### Publish

x86_64 `dotnet publish -c Release -r win-x64 -o ./bin/Publish --self-contained`

x86_32 `dotnet publish -c Release -r win-x86 -o ./bin/Publish --self-contained`

Arm64 `dotnet publish -c Release -r win-arm64 -o ./bin/Publish --self-contained`

You may face some link bugs. Use old unlink one to replace the dll in publish/

```
DirectWriteForwarder.dll
Vanara.PInvoke.Shared.dll
Vanara.PInvoke.User32.dll
```

##### X86 Addition Operation

Open `x86 Native Tools Command Prompt for VS 2019`

```cmd
cd path_to\ErogeHelper\bin\Publish
editbin /largeaddressaware ErogeHelper.exe
```

### Install

For users please run ErogeHelper.Installer.exe to register EH in windows context menu (aka right click menu).
