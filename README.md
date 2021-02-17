<p align="center">
  <img width="150px"  src="Docs/Logo.svg" />
</p>

<h1 align="center">数独生成</h1>
<p align="center">人工智能课程设计，使用 C#、WPF、.NET Core 制作的数独生成器</p>

## 简介

这是一个用来生成数独的程序，附带解数独的功能。

程序有一个看得过去的用户界面（下面有截图），使用回溯算法对数独求解。

第一次用 C#，代码写的比较一般，建议仅用于学习参考。

## 功能

已经开发完成的功能：

- 🔩 **生成数独**

    根据算法生成一个有解的数独（不一定只有一个解）。

- ⌛ **求解与验证数独**

    对程序中输入的数独进行求解，如果数独有多个解，则只能求出其中的一个解。

- 📁 **导入与导出**

    支持以 CSV 文件为格式的导入与导出。

- 🎮 **玩数独**

    支持手动填入数独（正常地进行数独游戏）。

- 📦 **其他功能** 

    还有一个并不完善的单步撤销的功能。

## 下载

课程演示的版本可以在 [Release](https://github.com/Lifeni/sudoku-generator/releases/tag/1.0) 中下载。

## 截图演示

程序部分截图及功能演示：

<details>
     <summary>主界面</summary>

![主界面](Docs/主界面.png)

</details>

下面都是 GIF，可以展开查看：

<details>
     <summary>生成与求解</summary>

![生成与求解](Docs/生成与求解.gif)

</details>

<details>
     <summary>文件导入</summary>

![文件导入](Docs/文件导入.gif)

</details>

<details>
     <summary>删除与撤销</summary>

![删除与撤销](Docs/删除与撤销.gif)

</details>

<details>
     <summary>文件导出</summary>

![文件导出](Docs/文件导出.gif)

</details>

<details>
     <summary>数独无解</summary>

![数独无解](Docs/数独无解.gif)

</details>

<details>
     <summary>做数独</summary>

![做数独](Docs/做数独.gif)

</details>

<details>
     <summary>设置初盘</summary>

![设置初盘](Docs/设置初盘.gif)

</details>

## 兼容性

程序运行需要 .NET Core 3.1 的支持，如果你的电脑上没有这个包，那么第一次运行的时候会提示安装。

经过虚拟机测试，在安装过 .NET Core 3.1 的 Windows 10 设备上可以正常运行。

测试后发现，程序在 Windows 7 上无法打开，即使安装了依赖包， 具体原因未知，可能是一个 Bug。

## 开源协议

MIT License
