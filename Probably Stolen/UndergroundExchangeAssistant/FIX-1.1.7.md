# 材料单不显示：1.1.7

本次 Player.log 确认 1.1.6 已加载，未出现前次读档的异常刷屏；用户反馈是桶身不显示材料单。

直接读取当前游戏 sharedassets1.assets，RenderNodePrefab 的组件默认值为：Image.enabled=false，RectMask2D.enabled=false，TreeNodeRender.enabled=true。TreeNodeRender.InstantiateAsRoot 初始化组件引用但不启用 Image；原版 GameItemElement 构造函数则显式设置 background.image.enabled=true。

模组换用该工厂后漏掉了启用 Image。1.1.7 在创建入口、取得 node.image 后显式启用它，保留当前素材、轮廓、解锁条件和交互逻辑。

验证：verify_render_node.py 对真实资源默认值的检查；正式版 Release 构建；原有自动测试。尚未进行修正版游戏内显示验收。不能将这一项图像显示修复等同于证明此前所有读档问题均已解决。
