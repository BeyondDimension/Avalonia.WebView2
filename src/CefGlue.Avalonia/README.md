# CefGlue.Avalonia 的 Git 子模块源码 Link 项目
https://github.com/BeyondDimension/CefGlue

在需要引用的项目 csproj 文件中末尾加入以下内容：
```
<Import Project="$(MSBuildThisFileDirectory)..\..\ref\CefGlue.Avalonia.props"/>
```