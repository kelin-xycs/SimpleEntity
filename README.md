# SimpleEntity
一个 用 C# 实现的 简单的 持久层 Entity 实现 。


这是一个 用 C# 实现的 简单的 持久层 Entity 实现 。

和 其它的 持久层 框架 不同 ， Simple Entity 并不打算 提供一个 抽象层 来 封装 和 隐藏 关系数据库 。
相反 ， 在 SimpleEntity 的 设计 里 ， 应用程序 和 关系数据库 之间 是 完全 公开 透明 自由 的 。 SimpleEntity 提供 对象 的 方式 来 访问 数据库 ， 帮助 开发 更便捷 ， 设计 更清晰（结构化 对象化） 。

SimpleEntity 提供了 一个 DbContext 类 。

DbContext 类 包含 Save(object entity) , Update(object entity) , UpdateNull(string[] columnNames, object id) ,
Delete(object id) , Flush() , Get(object id) , List Get(object[] ids) 方法 。

Save(object entity) 方法 将 entity Insert 到 数据库 表 中 ；

Update(object entity) 方法 Update entity 对应 的 表 资料 ， 只 Update 不为 null 的 Property ， 换句话说 ， 要 Update 哪个（哪些） 栏位 ， 就对 对应 的 Property 赋值 就行 。 不 Update 的 Property 不赋值 。

UpdateNull(string[] columnNames, object id) 方法 用于 将 指定 的 栏位 Update 为 null ，
有些时候， 需要 将 某些 栏位 Update 为 null ， 就 可以 使用 UpdateNull() 方法 。

Delete(object id) 方法 删除 表 资料 。 参数 id 为 主键 。 只支持 单一列 作为 主键 。 不支持 多列联合主键 。
这也是 OR Mapping 里的 一个 惯例 吧 。

Flush() 方法 将 Save() Update() UpdateNull() Delete() 方法 的 更新结果 提交 到 数据库 。
只有 Flush() 方法 才会 访问 数据库 。 Save() Update() UpdateNull() Delete() 方法 只是 将 更新 转换成 Sql 和 Sql参数 保存起来 。
在 Flush() 的 时候 才 会 批量（Batch）提交 到 数据库 。

Get(object id) 方法 根据 id 返回 entity ， id 是 主键 。

List Get(object[] ids) 方法 根据 ids 返回 entityList ， ids 是 一组 主键 。 这样可以 一次 返回 多个 主键 的 entity 对象 。

DbContext 是 线程级 的 操作 。 是 线程 安全 的 。 就是说， 在 一个 线程 里 ， 可以在 任意个地方 创建任意个 new DbContext() 对象 。 效果 都 一样 。 相同 DB 配置（Connection String， Provider） 的 DbContext 的 更新操作 ， 最终都会在 Flush() 方法 里 提交到 数据库 。

为了 更 清楚 的 说明 这一点 ， 我们 看一下 这部分 的 实现原理 ：

DbContext 实际的 操作 是 调用 SqlClientManager ， SqlClientManager 是 实现 IActiveRecordManager 接口 的 一个类 。

SqlClientManager 会把 Save Update Delete 操作 转换 为 Sql 和 Sql参数 ， 分别 存在 _sqlList , _paraList 两个 静态变量 里 。 这 2 个 静态变量 是 [ThreadStatic] 标记 标注 的 静态变量 。 就是说， 它们 和 普通的 静态变量 不同， 是 线程级 的 静态变量 。 每个 线程 都有 自己的 这 2 个 静态变量 。

从上面可以看出 ， 目前 SimpleEntity 只 支持 SqlServer , 如果 想 支持 别的 数据库 ， 比如 Oracle ， 那么 可以 新增一个 类 OracleClientManager ， 实现 IActiveRecordManager 接口 ， 然后 ， 把 SqlClientManager 拷贝 过去 稍微 修改 一下 就可以了。 啊 哈哈哈

关于 事务（Transaction） ， 可以使用 TransactionScope（System.Transactions.TransactionScope） 。 将 Flush() 放在 TransactionScope 中 就行 。

可以在 Demo 项目 中 查看 Demo ， Test.aspx 是 测试 Save Update 等 各种操作 的 页面 ， ManageDB.aspx 是 管理 测试 DB 的 页面 ， 可以执行 Sql 。

因为 项目 使用 的 是 Asp.net 内置 SqlServer （LocalDb） ， 所以 用 一个 ManageDB.aspx 页面 来 管理 DB 。

Demo 项目 是 一个 “Asp.net Web 窗体 项目” ， 这个 内置 SqlServer 就是 创建项目 时 自动生成 的 。 mdf ldf 文件 存放在 项目 的 App_Data 文件夹 里 。 创建项目 时 还自动生成了 很多 其它 的 东西 ， 我 把 那些 不需要 的 部分 删除 了 。






