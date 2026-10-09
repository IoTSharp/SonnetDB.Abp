# 已导入、待迁移的官方示例

这里保留首批 S02–S13 的完整官方源码，共 12 个例子、1662 个原文件。S01 已在 ../BookStore；总清单与迁移顺序见 [示例索引](../README.md) 和 [ROADMAP](../../ROADMAP.md)。

每个例子的原始文件保持 Git blob 字节，另附 MIT 来源许可证、SHA-256/Git blob 清单及 ADAPTATION.md。上游 README、旧包版本和数据库配置是来源记录；当前状态只表示源码已导入，不表示还原、构建或 SonnetDB 运行通过。

进入各自迁移任务时，再统一固定依赖、接入本仓库适配组件，并把可运行项目与实际测试纳入根解决方案/CI。保留完整业务与 UI，数据库/通用 Provider 能力缺口交给 SonnetDB issue，不修改 ABP 框架来绕过。
