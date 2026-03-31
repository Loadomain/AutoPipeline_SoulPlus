# AutoExtractor 外部调用说明书 (CLI Manual)

AutoExtractor 现在不仅可以作为带有界面的日常解压工具使用，也可以被你的其他软件乃至批处理脚本 (Batch/Shell) 自动调用执行其核心能力：智能地给隐藏后缀的压缩包附带正确后缀名并拆解、并且连同正常的深层次压缩包一并拆解删除原外壳。

## 基本说明

一旦启动指令中带有参数，程序会自动进入 **静默控制台模式 (CLI Mode)**。在这个模式下，它具备以下特性：
- **没有弹窗**：所有的输出和交互都被控制台文本输出（标准输出）所接管，绝不弹窗阻塞外部软件。
- **自动退出**：完成后工具自动终止生命周期并返回 Exit Code 0 告知宿主完成（如遇异常则非 0）。
- **密码本轮询**：即使遇到加密包，也会直接尝试使用配置时提供的所有潜在密码进行攻击式轮询解压，由于是自动化环境，它会在所有密码错误时直接将该加锁文件列入黑名单跳过，防止发生系统级的死循环。

## 参数列表

| 参数选项 | 说明 / 规则 | 举例 |
| :--- | :--- | :--- |
| `--dir` | (必填) 指定需要进行循环扫描及解压的根目录绝对路径 | `--dir "D:\Data\Downloads"` |
| `--target-ext`| (必填) 发现无后缀 / 伪后缀文件时，应该被强行修补上的目标后缀名 | `--target-ext .zip` |
| `--fake-ext` | (可选) 用来指定匹配“伪造的后缀”。如果填 `.mp4`，所有的被扫描的该后缀将被视为压缩包。不填该项默认只扫描没有后缀名的孤儿文件。 | `--fake-ext .mp4` |
| `--password` | (可选) 提供已知可能会用作加密包密码的值。外部程序允许传递若干个这样的参数来构建轮询池。 | `--password default123 --password 888888` |
| `--help`, `-h` | 打印简单的控制台帮助帮助信息。 | `--help` |

## 调用范例

### 1. 扫描没有后缀名的所有隐藏文件，并当作 .zip 解压
```bash
AutoExtractor.exe --dir "C:\MyFiles\Hidden" --target-ext .zip
```

### 2. 扫描并且强行修改所有的 mp4 到 rar，同时提供两个可能的密码
```bash
AutoExtractor.exe --dir "D:\Data" --fake-ext .mp4 --target-ext .rar --password "123456" --password "qwer"
```

## 被动调用集成 (C# Example)

如果是使用别的 C# 软件调用该控制台可以采用以下 `Process` 启动代码：
```csharp
using System.Diagnostics;

var proc = new Process();
proc.StartInfo.FileName = @"E:\loadfield\workforce\software\自动递归改压缩文件后缀\bin\Debug\net10.0-windows\AutoExtractor.exe";
proc.StartInfo.Arguments = "--dir \"D:\\Work\" --target-ext .zip --password \"guess1\"";
proc.StartInfo.UseShellExecute = false;

proc.Start();
proc.WaitForExit();

if (proc.ExitCode == 0) {
    Console.WriteLine("自动解压成功！");
}
```
