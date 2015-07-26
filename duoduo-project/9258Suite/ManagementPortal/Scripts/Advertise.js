var advertiseManager;
var imageTypes;

function Init(gridTagId) {

    $.ajax({
        url: 'Home/GetImageTypes',
        type: 'GET',
        cache: false,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.Success) {
                imageTypes = data.Rows;
                InitGrid(gridTagId);
            }
            else {
                alert('失败！' + data.Message);
            }
        },
        error: function (error) {
            alert(error);
        }
    });
}

function InitGrid(tagId)
{
    advertiseManager = $(gridTagId).ligerGrid({
        columns: [
        //{ display: 'ID', name: 'Id', width: 30, type: 'int', editor: { type: 'int' }, IsFrozen: true, isSort: false },
        { display: '名称', name: 'Name', width: 100, type: 'string', editor: { type: 'text' }, isSort: false },
        { display: '链接', name: 'Link', width: 200, type: 'string', editor: { type: 'text' }, isSort: false },
        {
            display: '图片类别', name: 'ImageType_Id', type: 'int', width: 150, isSort: false, editor: { type: 'select', data: imageTypes, valueField: 'Id', textField: 'Name' },
            render: function (item) {
                for (var i = 0; i < imageTypes.length; i++) {
                    if (imageTypes[i].Id == item.ImageType_Id) {
                        return imageTypes[i].Name;
                    }
                }
                return '';
            }
        },
          {
              display: '图片', name: 'Id', width: 80, isSort: false, render: function (rowdata, rowindex, value) {
                  var h = "";
                  if (!rowdata._editing) {
                      
                          h += " <a href='javascript:updateImage(" + rowdata.Id + ',' + rowindex + ")'>查看/修改</a> ";
                      
                  }
                  return h;
              }
          },
          {
              display: '操作', isSort: false, width: 80, render: function (rowdata, rowindex, value) {
                  var h = "";
                  if (!rowdata._editing) {
                      h += " <a href='javascript:beginEdit(advertiseManager," + rowindex + ")'>修改</a> ";
                      h += " <a href='javascript:deleteRow(advertiseManager," + rowindex + ")'>删除</a> ";
                  }
                  else {
                      h += " <a href='javascript:endEdit(advertiseManager," + rowindex + ")'>提交</a> ";
                      h += " <a href='javascript:cancelEdit(advertiseManager," + rowindex + ")'>取消</a> ";
                  }
                  return h;
              }
          }
        ],
        url: "Home/GetAdvertise",
        pageSizeOptions: [20, 40, 80], pageSize: 40, width: '95%', height: '95%',
        dataAction: "server",
        enabledEdit: true, clickToEdit: false, IsScroll: true, rownumbers: false, checkbox: true, allowAdjustColWidth: false,
        toolbar: {
            items: [{ text: '自定义查询', click: function () { search(advertiseManager); }, icon: 'search2' },
                { text: '添加广告', click: function () { addNewRow(advertiseManager, 'NewAdvertise',true); } },
                { text: '保存修改', click: function () { saveAll(advertiseManager, 'SaveAdvertise'); } },
                { text: '删除选中纪录', click: function () { deleteSelected(advertiseManager); } }]
        }
    });
}

function updateImage(id, index) {
    uploadImage('Home/UploadImage/Image/' + id + '/'+id, index);
}