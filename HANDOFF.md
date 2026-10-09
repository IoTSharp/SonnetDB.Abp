# SonnetDB.Abp 交接记录

日期：2026-10-09（Asia/Shanghai）。工作区：D:/source/SonnetDB.Abp。

用户已授权创建 ABP 独立仓库并在此对接。已创建公开远端 https://github.com/IoTSharp/SonnetDB.Abp，origin 指向该仓库。当前分支 main；此交接随本次初始化内容提交保存，精确提交标识以 git log -1 为准。

初始代码提交 fb22306ef0b03bae10a05c085b6cfff0116697db 已推送 main，并通过独立 GitHub branch API 回读核对。该提交的 Ubuntu CI 37906959016 已 completed/success；本段更新为仅文档的验证记录，最新提交由 git log -1 读取。

开始时此目录不存在，因此在修改产品源码前建立此交接。SonnetDB 主仓库的 Workbench/博客及其它工作树变更不属于本项目。

## 已完成与验证

- 三项目：SonnetDB.Abp.EntityFrameworkCore 适配包、标准 ABP/Autofac 控制台样例、xUnit 集成测试。通过公开 NuGet 固定 SonnetDB.EntityFrameworkCore 4.0.0、ABP 10.7.0、EF Core 10.0.12；没有跨仓库 ProjectReference。
- 默认/指定 DbContext 的 UseSonnetDB、动态连接字符串传递、ExistingConnection 优先复用。真实双上下文测试验证同一个连接/底层事务及共同回滚。
- 本地 Release 构建零警告/零错误，微试 1/1，完整集成 8/8，无跳过。CRUD/分页、审计、软删除启停、宿主/两租户过滤、并发戳、显式及未完成 UOW 回滚均通过。样例实际写入/读取成功；使用 EnsureCreated，不是 migrations。
- 初始化 GitHub 仓库已由独立 GET 回读核实；尚未发布 NuGet 或提交上游 PR。README/ROADMAP/CHANGELOG/AGENTS/架构/兼容性和 CI 已建立。
- 最终完整 restore、原级别 format、Release build/test 均退出 0；构建零警告/错误，最终 8/8 无跳过。实际退出结果保存在忽略的 artifacts/checks/*-final.json；提交/推送结果以实际 Git 与远端分支为准，GitHub CI 状态需读取实际 run，不由本地 PASS 推断。

## 下一顺序与边界

先核 git status、origin/main 与最新 CI，再推进 A02 的标准迁移和模型边界；其后 A03 真实 Server/独立租户库、A04 内置模块。ABP Identity/Permission/Setting/OpenIddict、复杂查询、恢复/长稳/Native AOT/正式包未验，不从本地八项推导。

通用 Provider/引擎问题回 SonnetDB 单独修复；本仓库只维护 ABP 适配。许可证和适配包发布策略尚待维护者确定，不自动发布。用户的创建仓库授权涵盖本轮初始化提交/推送，不涵盖额外聊天、自动化或外发消息。

## 资源与失败保留

命令通过有界 PS7 包装器执行，数量/墙钟限制和身份记录明确；只回收核验归属的完整进程树。各已执行命令的 recorded remaining 见 JSON，未观察后代整体不存在未证明。测试专属 Temp 数据库已由 finally 校验所有权与绝对路径后移除；样例数据库和本地日志保留在忽略的 artifacts，未提交。

首次短命 dotnet 微试缺身份、第二次 PowerShell 路径多匹配、首轮遗漏 Modeling using、普通 DI 微试和 GitHub 直连/gh 代理错误均保留。修正路径单选/短命退出记录后微试通过；GitHub REST 经明确代理成功，凭据仅进程内变量、不进入日志或仓库。无新下载工具、服务或共享缓存清理。
