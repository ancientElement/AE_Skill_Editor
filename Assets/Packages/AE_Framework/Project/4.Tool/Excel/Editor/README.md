1. Excel文件应该放置在ArtRes/Excel当中
  如果想要修改 就去修改ExcelTool当中的EXCELPATH路径变量
2. 配置表规则
  第一行：字段名
  第二行：字段类型（字段类型一定不要配置错误，字段类型目前只支持int float bool string)
  如果想要再添加类型，需要在ExcelTool的GenerateExcelBinary方法中
  和BinaryDataMgr的LoadTable方法当中对应添加读写的逻辑
  第三行：主键是哪一个字段 需要通过key来标识主键
  第四行：描述信息（只是给别人看，不会有别的作用）
  第五行~第n行：就是具体数据信息
  下方的表名决定了数据结构类，容器类，2进制文件的文件名
3. 生成的容器类和数据结构类可以在ExcelTool当中侈改DATA CLASS PATH和DATA CONTAINER PAT
  变量来进行更改
4. 生成2进制文件的路径可以在BinaryDataMgr当中的DATA BINARYPATH变量来进行修改