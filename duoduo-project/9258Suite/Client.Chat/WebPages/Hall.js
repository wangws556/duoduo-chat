﻿
var allTopLevelRoomGroups;
var searchResult = new Array();
var currentTabIndex = 0;
var roomGroupTab;
var roomGroupTabHeader;
var roomGroupTrees = new Array();
var rankSplitters = new Array();
var roomGroupSplitters = new Array();
var roomsPanels = new Array();
var rankPanels = new Array();
var searchTexts = new Array();
var searchButtons = new Array();
var locations = new Array();
var roomGroupTreeDockPanels = new Array();
var roomGroupDockPanels = new Array();
var rankDockPanels = new Array();
var rankTabs = new Array();
var originalRankPanel;
//var loginWindow;
var loginingWindow;
var userName;
var password;
var remember;
var iconPath;
var autologin;
var loginButton;
var globalSlides;
var globalCompanyPics;
var globalAdvPics;
var iCountOfSlidesImage;
var iSlidesPreIndex = 0;
var slidesTimer;

var iCountOfAdvImage;
var iAdvPreIndex = 0;
var advTimer;

function login() {
    //loginWindow.jqxWindow('close');
    //loginingWindow.jqxWindow('open');
    window.external.Login(userName.val(), password.val(), remember.prop('checked'), icon.val(), autologin.prop('checked'));
}

function cancelLogin() {
    //loginingWindow.jqxWindow('close');
    //loginWindow.jqxWindow('open');
}

function register() {
    window.external.Register();
}

function cancel() {
    window.external.Close();
}

function navigate(topRoomGroupId, rg, r) {
    var roomGroupTree = GetElement(topRoomGroupId, roomGroupTrees);
    var li = roomGroupTree.find("li:has(div:contains('" + rg.name + "'))").last().get(0);
    roomGroupTree.jqxTree('expandItem', li);
    roomGroupTree.jqxTree('selectItem', li);

    var roomsPanel = GetElement(topRoomGroupId, roomsPanels);
    var roomDiv = roomsPanel.find("div[roomId='" + r.id + "']").first();
    roomsPanel.empty();
    roomDiv.appendTo(roomsPanel);
    //var bg = roomDiv.css('background-color');
    //roomDiv.animate({ backgroundColor: 'lightgreen' }, 2000, function () {
    //    roomDiv.animate({ backgroundColor: bg }, 2000);
    //});
}

function searchRoomGroup(topRoomGroupId, rg, val) {
    if (val) {
        if (rg.nodes && rg.nodes.length > 0) {
            for (var i = 0; i < rg.nodes.length; i++) {
                var r = rg.nodes[i];
                if (isNaN(val)) {
                    if (r.name.indexof(val) >= 0) {
                        navigate(topRoomGroupId, rg, r);
                        return true;
                    }
                }
                else {
                    var valString = val.toString();
                    var idString = r.id.toString();
                    if (idString.indexOf(valString) >= 0
                        || r.name.indexOf(valString) >= 0) {
                        navigate(topRoomGroupId, rg, r);
                        return true;
                    }
                }
            }
        }
        if (rg.items && rg.items.length > 0) {
            for (var i = 0; i < rg.items.length; i++) {
                if (searchRoomGroup(topRoomGroupId, rg.items[i], val)) {
                    return true;
                }
            }
        }
    }
    return false;
}

function searchRooms(val) {
    if (val) {
        var rg = allTopLevelRoomGroups[currentTabIndex];
        var topRoomGroupId = rg.id;
        if (searchRoomGroup(topRoomGroupId, rg, val)) {

        }
        else {
            alert('没有您要搜索的房间！');
        }
    }
}

function enterRoom(id)
{
    window.external.EnterRoom(id);
}

function selectRoomGroup(rg) {
    if (rg) {
        var id = allTopLevelRoomGroups[currentTabIndex].id;
        var location = GetElement(id, locations);
        location.text(rg.url);
        var roomsPanel = GetElement(id, roomsPanels);
        clearInterval(slidesTimer);
        iSlidesPreIndex = 0;
        clearInterval(advTimer);
        iAdvPreIndex = 0;
        roomsPanel.empty();
        if (rg.nodes) {
            var rooms = rg.nodes;
            var roomPage = $("<div style='display:inline'></div>");
            for (var i = 0; i < rooms.length; i++) {
                var divOutter = $("<div class='outerRoomDiv'></div>");
                var div = $("<div class='roomBlock'></div>");
                //var divPlay = $("<div class='playBtn' id='playBtn"+rooms[i].id+"'></div>");

                //divPlay.append("<span>房间Id：" + rooms[i].id + "</span>");
                divOutter.attr("roomId", rooms[i].id);
                //divPlay.append("<a href='javascript:enterRoom(" + divOutter.attr("roomId") + ")' title='进入房间'></a>");
                div.append("<div><img class='roomImage' onclick='javascript:enterRoom(" + divOutter.attr("roomId") + ")' style='cursor:pointer' src='" + rooms[i].icon + "'/></div>");
                div.append("<div class='roomText'><span>房间名：" + rooms[i].name +
                    "<span>人数：" + rooms[i].count + "/" + rooms[i].maxcount + "</span>"
                    + "<span>房间Id：" + rooms[i].id + "</span>");


                //divOutter.mouseenter(function () {
                //    $("#playBtn" + $(this).attr("roomId")).css("display", "block");
                //});
                //divOutter.mouseleave(function () {
                //    $("#playBtn" + $(this).attr("roomId")).css("display", "none");
                //});
                div.appendTo(divOutter);
                //divPlay.appendTo(divOutter);
                divOutter.appendTo(roomPage);
            }

            roomPage.appendTo(roomsPanel);

            var advDiv = Advertise(globalAdvPics);
            RunAdvs();
            advDiv.appendTo(roomsPanel);
        }
        else {
            clearInterval(slidesTimer);
            iSlidesPreIndex = 0;
            clearInterval(advTimer);
            iAdvPreIndex = 0;
            var roomGPage = RoomGroupPage(rg);
            if (roomGPage) {
                RunAdvs();
                roomGPage.appendTo(roomsPanel);
            }
        }
    }
}

function RoomGroupPage(roomG)
{
    if(roomG && roomG.items)
    {
        var resultDiv = $("<div></div>");
        var onlines = new Array();
        var roomGroups = roomG.items;
        for (var i = 0; i < roomGroups.length; i++)
        {
            var rooms = roomGroups[i].nodes;
            if(rooms)
            {
                for(var j = 0; j<rooms.length; j++)
                {
                    var num = parseInt(rooms[j].count);
                    if(num > 0)
                        onlines.push(rooms[j]);
                }
            }
        }
        var roomsDiv = $("<div style='display:inline'></div>");
        if (onlines.length > 0) {
            onlines = onlines.sort(function (a, b) {
                return b.count - a.count;
            })
            
            for(var i = 0;i<onlines.length && i<24;i++)
            {
                var divOutter = $("<div class='outerRoomDiv'></div>");
                var div = $("<div class='roomBlock'></div>");
             
                divOutter.attr("roomId", onlines[i].id);
                
                div.append("<div><img class='roomImage' onclick='javascript:enterRoom(" + divOutter.attr("roomId") + ")' style='cursor:pointer' src='" + onlines[i].icon + "'/></div>");
                div.append("<div class='roomText'><span>房间名：" + onlines[i].name +
                    "<span>人数：" + onlines[i].count + "/" + onlines[i].maxcount + "</span>"
                    + "<span>房间Id：" + onlines[i].id + "</span>");

                div.appendTo(divOutter);
                divOutter.appendTo(roomsDiv);
            }
        }
        roomsDiv.appendTo(resultDiv);
        var adv = Advertise(globalAdvPics);
        adv.appendTo(resultDiv);
        return resultDiv;
    }
}

function forgetPassword() {

}

function LoginSucceed() {
    //loginingWindow.jqxWindow('close');
    //loginWindow.jqxWindow('close');
    //roomGroupTab.attr('disabled', false);
}

function SwitchUser()
{
    //loginWindow.jqxWindow('open');
}

function SetLoginUser(id, pwd) {
    userName.val(id);
    password.val(pwd);
}

function GetElement(id, dic) {
    for (var i = 0; i < dic.length; i++) {
        if (dic[i].id == id) {
            return dic[i].element;
        }
    }
    return null;
}

function autologinclick() {
    if (autologin.prop('checked'))
        remember.prop('checked', true);
    else
        remember.prop('checked', false);
}

function rememberclick() {
    if (autologin.prop('checked'))
        remember.prop('checked', true);
}

function InitGlobalAdvPage(slides, scrolls, pic) {
    globalSlides = eval("(" + slides + ")");
    globalCompanyPics = eval("(" + scrolls + ")");
    globalAdvPics = eval("(" + pic + ")");
    iCountOfSlidesImage = globalSlides.length;
    iCountOfAdvImage = globalAdvPics.length;
    if (globalSlides != null && globalCompanyPics != null && globalAdvPics != null)
    {
        var div = GlobalAdvPage(globalSlides, globalCompanyPics, globalAdvPics);
        var id = allTopLevelRoomGroups[0].id;
        var roomsPanel = GetElement(id, roomsPanels);
        roomsPanel.empty();
        div.appendTo(roomsPanel);
        RunSlides();
        //RunScroll();
        RunAdvs();
    }
}

function SlidesTimer() {
    var iNextIndex = (parseInt(iSlidesPreIndex) + 1) % iCountOfSlidesImage;
    $(".ppt-container ul.button-list li img[data-index=" + iNextIndex + "]").click();
}

function AdvTimer() {
    var iNextIndex = (parseInt(iAdvPreIndex) + 1) % iCountOfAdvImage;
    $(".ppt-container-company ul.button-list li img[data-index=" + iNextIndex + "]").click();
}

function RunSlides()
{
    slidesTimer = setInterval(SlidesTimer, 2000);

}

function RunAdvs()
{
    advTimer = setInterval(AdvTimer, 2000);
}

//var _slide;
//var _slideli1;
//var _slideli2;


//function Marquee() {
//    if (_slide.scrollLeft() >= _slideli1.width())
//        _slide.scrollLeft(0);
//    else {
//        _slide.scrollLeft(_slide.scrollLeft() + 1);
//    }
//}

//function RunScroll() {
//    _slide = $("#slide");
//    _slideli1 = $(".slideli1");
//    _slideli2 = $(".slideli2");
//    _slideli2.html(_slideli1.html());
   
//        //两秒后调用
//        var sliding = setInterval(Marquee, 30)
//        _slide.hover(function () {
//            //鼠标移动DIV上停止
//            clearInterval(sliding);
//        }, function () {
//            //离开继续调用
//            sliding = setInterval(Marquee, 30);
//        });
   
//}


//function scrollPage(pics) {
    
//    var slideDiv = $("<div id='slide' style='overflow:hidden;width:900px;margin:0 auto;'></div>");
//    var table = $("<table class='slideul1' style='width:900px;' ></table>");
//    var tr1 = $("<tr ></tr>");
//    var td1 = $("<td class='slideli1'></td>");
//    var table2 = $("<table></table>");
//    var tr2 = $("<tr></tr>");

//    for (var i = 0; i < 10; i++) {
//        tr2.append($("<td><a href='"+pics[0].Link+"' target='_blank'><img style='width:180px;height:180px' src='" + pics[0].StaticImageFile + "' alt=''></img></a></td>"));
//    }

//    var td2 = ($("<td class='slideli2'></td>"));
//    tr2.appendTo(table2);
//    table2.appendTo(td1);
//    td1.appendTo(tr1);
//    td2.appendTo(tr1);
//    tr1.appendTo(table);
//    table.appendTo(slideDiv);
   
//    return slideDiv;
//}

function CompanyAdvertise(pics)
{
    var div = $("<div style='display:inline;padding-top:20; width:650'></div>");
    for(var i = 0; i< pics.length; i++)
    {
        var a = $("<a href = '" + pics[i].Link + "' target='_blank'></a>");
        var img = $("<img src='" + pics[i].StaticImageFile + "' style ='width:200px;height:180px; border:0; margin:10 10 0 0;' alt=''></img>");
        img.appendTo(a);
        a.appendTo(div);
    }
    return div;
}

function Advertise(pics)
{
    var div1 = $("<div class='ppt-container-company'></div>");
    var imgUl = $("<ul class='image-list'></ul>");
    for (var i = 0; i < pics.length; i++) {
        var li = $("<li data-index='" + i + "'></li>");
        var a = $("<a href = '" + pics[i].Link + "' target='_blank'></a>");
        var img = $("<img src='" + pics[i].StaticImageFile + "' style ='height:180; width:650; border:0;margin:10 5 0 0' alt=''></img>");
        if (i != 0) {
            li.addClass('hide');
        }
        img.hover(function () {
            //鼠标移动DIV上停止
            clearInterval(advTimer);
        }, function () {
            //离开继续调用
            advTimer = setInterval(AdvTimer, 2000);
        });
        img.appendTo(a);
        a.appendTo(li);
        li.appendTo(imgUl);
    }

    var btnUl = $("<ul class='button-list'></ul>");
    for (var i = 0; i < pics.length; i++) {
        var li = $("<li></li>");
        var img = $("<img data-index='" + i + "' src='" + pics[i].StaticImageFile + "' alt=''></img>")
        if (i == 0) {
            img.addClass('selected');
        }

        img.on('click', CompanyBtnClick);

        img.hover(function () {
            //鼠标移动DIV上停止
            clearInterval(advTimer);
            var iIndex = $(this).attr("data-index");
            $(".ppt-container-company ul.button-list li img[data-index=" + iIndex + "]").click();

        }, function () {
            //离开继续调用
            advTimer = setInterval(AdvTimer, 2000);
        });
        img.appendTo(li);
        li.appendTo(btnUl);
    }
    imgUl.appendTo(div1);
    btnUl.appendTo(div1);
    return div1;
}

function SlidesBtnClick() {
    var iIndex = $(this).attr("data-index");
    if (iIndex == iSlidesPreIndex) {
        return; // 点击了当前图片，不切换 
    }

    $(".ppt-container .image-list li[data-index=" + iIndex + "]").fadeIn(1000);
    $(".ppt-container .image-list li[data-index=" + iSlidesPreIndex + "]").fadeOut(1000);
    iSlidesPreIndex = iIndex;
    $(".ppt-container .button-list img").removeClass("selected");
    $(this).addClass("selected");
}

function CompanyBtnClick() {
    var iIndex = $(this).attr("data-index");
    if (iIndex == iAdvPreIndex) {
        return; // 点击了当前图片，不切换 
    }

    $(".ppt-container-company .image-list li[data-index=" + iIndex + "]").fadeIn(1000);
    $(".ppt-container-company .image-list li[data-index=" + iAdvPreIndex + "]").fadeOut(1000);
    iAdvPreIndex = iIndex;
    $(".ppt-container-company .button-list img").removeClass("selected");
    $(this).addClass("selected");
}

function GlobalAdvPage(slides, scrolls, pics)
{
    var outterDiv = $("<div style='text-align:left;padding-left:5px;width:650px'></div>");
    var div1 = $("<div class='ppt-container'></div>");
    var imgUl = $("<ul class='image-list'></ul>");
    for(var i = 0; i<slides.length; i++)
    {
        var li = $("<li data-index='" + i + "'></li>");
        var a = $("<a href = '" + slides[i].Link + "' target='_blank'></a>");
        var img = $("<img src='" + slides[i].StaticImageFile + "' style ='height:350; width:695 border:0' alt=''></img>");
        if (i != 0)
        {
            li.addClass('hide');
        }
        img.hover(function () {
            //鼠标移动DIV上停止
            clearInterval(slidesTimer);
        }, function () {
            //离开继续调用
            slidesTimer = setInterval(SlidesTimer, 2000);
        });
        img.appendTo(a);
        a.appendTo(li);
        li.appendTo(imgUl);
    }

    var btnUl = $("<ul class='button-list'></ul>");
    for (var i = 0; i < slides.length; i++)
    {
        var li = $("<li></li>");
        var img = $("<img data-index='" + i + "' src='" + slides[i].StaticImageFile + "' alt=''></img>")
        if (i == 0)
        {
            img.addClass('selected');
        }

        img.on('click', SlidesBtnClick);

        img.hover(function () {
            //鼠标移动DIV上停止
            clearInterval(slidesTimer);
            var iIndex = $(this).attr("data-index");
            $(".ppt-container ul.button-list li img[data-index=" + iIndex + "]").click();

        }, function () {
            //离开继续调用
            slidesTimer = setInterval(SlidesTimer, 2000);
        });
        img.appendTo(li);
        li.appendTo(btnUl);
    }
    imgUl.appendTo(div1);
    btnUl.appendTo(div1);

    var div2 = CompanyAdvertise(scrolls);
    
    var div3 = Advertise(pics);
    
    div1.appendTo(outterDiv);
    div2.appendTo(outterDiv);
    div3.appendTo(outterDiv);
    return outterDiv;
}

function Back(event)
{
    var roomGroupTree = GetElement(allTopLevelRoomGroups[currentTabIndex].id, roomGroupTrees);
    roomGroupTree.jqxTree('collapseAll');

    var roomsPanel = GetElement(allTopLevelRoomGroups[currentTabIndex].id, roomsPanels);
    var location = GetElement(allTopLevelRoomGroups[currentTabIndex].id, locations);
    roomsPanel.empty();
    location.empty();
    var div = GlobalAdvPage(globalSlides, globalCompanyPics, globalAdvPics);
    div.appendTo(roomsPanel);
    //RunScroll();
    RunSlides();
    RunAdvs();
}

function InitHallWithLogin(roomGroups,userId, pwd, remberPwd, autoLogin)
{
    userName = $('#userid');
    password = $('#password');
    remember = $('#remember');
    autologin = $('#autologin');
    loginButton = $('#loginBtn');

    //globalSlides = eval("(" + slides + ")");
    //globalScrolls = eval("(" + scrolls + ")");
    //globalPic = eval("(" + pic + ")");
    //loginingWindow = $('#loginingWindow').jqxWindow({
    //    theme: 'energyblue', width: 450, height: 250, zIndex: 99999, resizable: false, isModal: true, draggable: false,
    //    showCollapseButton: false, showCloseButton: false, autoOpen: false, position: { x: 350, y: 150 },
    //    showAnimationDuration: 10, closeAnimationDuration: 10
    //});
    allTopLevelRoomGroups = eval(roomGroups);
    originalRankPanel = $('#rankPanel');
    roomGroupTab = $('#roomGroupTab');
    roomGroupTabHeader = $('#roomGroupTabHeader');
    for (var i = 0; i < allTopLevelRoomGroups.length; i++) {
        roomGroupTabHeader.append("<li><p class='channelTabHeader'>" + allTopLevelRoomGroups[i].name + "</p></li>");
        var id = allTopLevelRoomGroups[i].id;
        var div =
            "<div id='rankSplitter" + id + "' >" +
                "<div>" +
                    "<div id='roomGroupSplitter" + id + "' >" +
                        "<div>" +
                            "<div id='roomGroupTreeDockPanel" + id + "'>" +
                                "<div dock='top' style='height:30px;'>" +
                                    "<input type='text' id='searchText" + id + "' class='searchBox' />" +
                                    "<input id='search" + id + "' class='rb1' type='button' value='搜索' onmouseover=\"this.className='rb2'\" onmouseout=\"this.className='rb1'\"></input>" +
                                "</div>" +
                                "<div><div id='roomGroupTree" + id + "' class='roomGroupTree' style='position:absolute;top:30;left:0'/></div>" +
                            "</div>" +
                        "</div>" +
                        "<div>" +
                            "<div id='roomGroupDockPanel" + id + "'>" +
                                "<div dock='top' style='height:30px;overflow-x:hidden;overflow-y:hidden;'>" +
                                    "<span id='location" + id + "' class='location'></span>" +
                                "</div>" +
                                "<div><div id='roomsPanel" + id + "' style='position:absolute;height:100%;width:100%;top:0;left:0;overflow-x:hidden;overflow-y:auto;'/></div>" +
                            "</div>" +
                        "</div>" +
                    "</div>" +
                "</div>" +
                "<div><div id='rankPanel" + id + "' style='position:absolute;height:100%;width:100%;top:0;left:0;'/></div>" +
            "</div>";
        roomGroupTab.append(div);
        rankSplitters.push({ id: id, element: $("#rankSplitter" + id) });
        roomGroupTreeDockPanels.push({ id: id, element: $("#roomGroupTreeDockPanel" + id) });
        roomGroupSplitters.push({ id: id, element: $("#roomGroupSplitter" + id) });
        searchTexts.push({ id: id, element: $("#searchText" + id) });
        searchButtons.push({ id: id, element: $("#search" + id) });
        roomGroupTrees.push({ id: id, element: $("#roomGroupTree" + id) });
        roomGroupDockPanels.push({ id: id, element: $("#roomGroupDockPanel" + id) });
        locations.push({ id: id, element: $("#location" + id) });
        roomsPanels.push({ id: id, element: $("#roomsPanel" + id) });
        rankPanels.push({ id: id, element: $("#rankPanel" + id) });

        //loginWindow = $('#loginWindow').jqxWindow({
        //    theme: 'energyblue', width: 450, height: 340, zIndex: 99999, resizable: false, isModal: true, draggable: false,
        //    showCollapseButton: false, showCloseButton: false, autoOpen: false, position: { x: 300, y: 200 },
        //    showAnimationDuration: 10, closeAnimationDuration: 10
        //});
       
    }
    roomGroupTab.jqxTabs({
        theme: 'energyblue', width: '100%', height: '100%', showCloseButtons: false, autoHeight: true, scrollable: true, scrollPosition: 'left',
        initTabContent: function (index) {
            var id = allTopLevelRoomGroups[index].id;

            var rankSplitter = GetElement(id, rankSplitters);
            rankSplitter.jqxSplitter({
                theme: 'energyblue', width: '100%', height: '100%', orientation: 'vertical', splitBarSize: 1, resizable: false,
                showSplitBar: true, panels: [{ size: '90%' }, { size: '10%', min: 230 }]
            });
            var roomGroupSplitter = GetElement(id, roomGroupSplitters);
            roomGroupSplitter.jqxSplitter({
                theme: 'energyblue', width: '100%', height: '100%', orientation: 'vertical', splitBarSize: 1, resizable: false,
                showSplitBar: true, panels: [{ size: 210, min: 210 }]
            });

            var roomGroupTreeDockPanel = GetElement(id, roomGroupDockPanels);
            roomGroupTreeDockPanel.jqxDockPanel({ theme: 'energyblue', width: '100%', height: '100%', lastchildfill: true });

            var searchText = GetElement(id, searchTexts);
            searchText.jqxInput({ theme: 'energyblue', placeHolder: "搜索房间Id或房间名" });
            var searchButton = GetElement(id, searchButtons);
            searchButton.jqxButton({ theme: 'energyblue' });
            searchButton.on('click', function () {
                var val = searchText.jqxInput('val');
                if (val) {
                    searchRooms(val)
                }
                else {
                    alert('请输入房间Id或房间名');
                }
            });

            var roomGroupTree = GetElement(id, roomGroupTrees);
            roomGroupTree.jqxTree({
                theme: 'energyblue', width: '100%', height: '95%',
                source: allTopLevelRoomGroups[index].items, toggleMode: 'click', toggleIndicatorSize: 16
            });

            roomGroupTree.on('select', function (e) {
                var args = e.args;
                var item = roomGroupTree.jqxTree('getItem', args.element);
                if (item) {
                    var rg = GetRoomGroup(item.value);
                    selectRoomGroup(rg);
                  
                }
            });

            var roomGroupDockPanel = GetElement(id, roomGroupDockPanels);
            roomGroupDockPanel.jqxDockPanel({ theme: 'energyblue', width: '100%', height: '100%', lastchildfill: true });

            var rankPanel = GetElement(id, rankPanels);
            rankPanel.html(originalRankPanel.html());
            var rankDockPanel = rankPanel.find('div.rankDockPanel');
            var rankTab = rankPanel.find('div.rankTab');
            rankDockPanels.push({ id: id, element: rankDockPanel });
            rankTabs.push({ id: id, element: rankTab });
            rankDockPanel.jqxDockPanel({ theme: 'energyblue', width: '100%', height: '100%', lastchildfill: true });
            rankTab.jqxTabs({ theme: 'energyblue', width: '100%', height: '100%', showCloseButtons: false, autoHeight: true });

            if (index == 0) {
                //window.external.WebPageContentLoaded();
            }
        }
    });
    roomGroupTab.on('selected', function (event) {
        
        currentTabIndex = event.args.item;
        var roomsPanel = GetElement(allTopLevelRoomGroups[currentTabIndex].id, roomsPanels);
        var div = GlobalAdvPage(globalSlides, globalCompanyPics, globalAdvPics);
        div.appendTo(roomsPanel);
        //RunScroll();
        RunSlides();
        RunAdvs();
    });

    roomGroupTab.on('unselected', function (event) {
        currentTabIndex = event.args.item;
        var roomsPanel = GetElement(allTopLevelRoomGroups[event.args.item].id, roomsPanels);
        var location = GetElement(allTopLevelRoomGroups[event.args.item].id, locations);
        var roomGroupTree = GetElement(allTopLevelRoomGroups[event.args.item].id, roomGroupTrees);
        roomGroupTree.jqxTree('collapseAll');
        clearInterval(slidesTimer);
        iSlidesPreIndex = 0;
        clearInterval(advTimer);
        iAdvPreIndex = 0;
        roomsPanel.empty();
        location.empty();
    });

    //roomGroupTab.attr('disabled', true);

    userName.val(userId);
    password.val(pwd);
    remember.prop("checked", remberPwd);
    autologin.prop("checked", autoLogin);
    //loginWindow.jqxWindow('open');

    //if (autoLogin == true) {
    //    login(userId, pwd, remberPwd, autoLogin);
    //}
}

function InitHall(roomGroups) {
    userName = $('#userid');
    password = $('#password');
    remember = $('#remember');
    autologin = $('#autologin');
    loginButton = $('#loginBtn');

    //loginingWindow = $('#loginingWindow').jqxWindow({
    //    theme: 'energyblue', width: 450, height: 250, zIndex: 99999, resizable: false, isModal: true, draggable: false,
    //    showCollapseButton: false, showCloseButton: false, autoOpen: false, position: { x: 300, y: 200 },
    //    showAnimationDuration: 10, closeAnimationDuration: 10
    //});

    //loginWindow = $('#loginWindow').jqxWindow({
    //    theme: 'energyblue', width: 450, height: 340, zIndex: 99999, resizable: false, isModal: true, draggable: false,
    //    showCollapseButton: false, showCloseButton: false, autoOpen: false, position: { x: 300, y: 200 },
    //    showAnimationDuration: 10, closeAnimationDuration: 10
    //});
    //loginWindow.jqxWindow('open');
    allTopLevelRoomGroups = eval(roomGroups);
    originalRankPanel = $('#rankPanel');
    roomGroupTab = $('#roomGroupTab');
    roomGroupTabHeader = $('#roomGroupTabHeader');
    for (var i = 0; i < allTopLevelRoomGroups.length; i++) {
        roomGroupTabHeader.append("<li><p class='channelTabHeader'>" + allTopLevelRoomGroups[i].name + "</p></li>");
        var id = allTopLevelRoomGroups[i].id;
        var div =
            "<div id='rankSplitter" + id + "' >" +
                "<div>" +
                    "<div id='roomGroupSplitter" + id + "' >" +
                        "<div>" +
                            "<div id='roomGroupTreeDockPanel" + id + "'>" +
                                "<div dock='top' style='height:30px;'>" +
                                    "<input type='text' id='searchText" + id + "' class='searchBox' />" +
                                    "<input id='search" + id + "' class='rb1' type='button' value='搜索' onmouseover=\"this.className='rb2'\" onmouseout=\"this.className='rb1'\"></input>" +
                                "</div>" +
                                "<div><div id='roomGroupTree" + id + "' class='roomGroupTree' style='position:absolute;top:30;left:0'/></div>" +
                            "</div>" +
                        "</div>" +
                        "<div>" +
                            "<div id='roomGroupDockPanel" + id + "'>" +
                                "<div dock='top' style='height:30px;overflow-x:hidden;overflow-y:hidden;'>" +
                                    "<span id='location" + id + "' class='location'></span>" +
                                "</div>" +
                                "<div><div id='roomsPanel" + id + "' style='position:absolute;height:100%;width:100%;top:0;left:0;overflow-x:hidden;overflow-y:auto;'/></div>" +
                            "</div>" +
                        "</div>" +
                    "</div>" +
                "</div>" +
                "<div><div id='rankPanel" + id + "' style='position:absolute;height:100%;width:100%;top:0;left:0;'/></div>" +
            "</div>";
        roomGroupTab.append(div);
        rankSplitters.push({ id: id, element: $("#rankSplitter" + id) });
        roomGroupTreeDockPanels.push({ id: id, element: $("#roomGroupTreeDockPanel" + id) });
        roomGroupSplitters.push({ id: id, element: $("#roomGroupSplitter" + id) });
        searchTexts.push({ id: id, element: $("#searchText" + id) });
        searchButtons.push({ id: id, element: $("#search" + id) });
        roomGroupTrees.push({ id: id, element: $("#roomGroupTree" + id) });
        roomGroupDockPanels.push({ id: id, element: $("#roomGroupDockPanel" + id) });
        locations.push({ id: id, element: $("#location" + id) });
        roomsPanels.push({ id: id, element: $("#roomsPanel" + id) });
        rankPanels.push({ id: id, element: $("#rankPanel" + id) });
    }

    roomGroupTab.jqxTabs({
        theme: 'energyblue', width: '100%', height: '100%', showCloseButtons: false, autoHeight: true, scrollable: true, scrollPosition: 'left',
        initTabContent: function (index) {
            var id = allTopLevelRoomGroups[index].id;

            var rankSplitter = GetElement(id, rankSplitters);
            rankSplitter.jqxSplitter({
                theme: 'energyblue', width: '100%', height: '100%', orientation: 'vertical', splitBarSize: 1, resizable: false,
                showSplitBar: true, panels: [{ size: '90%' }, { size: '10%', min: 230 }]
            });
            var roomGroupSplitter = GetElement(id, roomGroupSplitters);
            roomGroupSplitter.jqxSplitter({
                theme: 'energyblue', width: '100%', height: '100%', orientation: 'vertical', splitBarSize: 1, resizable: false,
                showSplitBar: true, panels: [{ size: 210, min: 210 }]
            });

            var roomGroupTreeDockPanel = GetElement(id, roomGroupDockPanels);
            roomGroupTreeDockPanel.jqxDockPanel({ theme: 'energyblue', width: '100%', height: '100%', lastchildfill: true });

            var searchText = GetElement(id, searchTexts);
            searchText.jqxInput({ theme: 'energyblue', placeHolder: "搜索房间Id或房间名" });
            var searchButton = GetElement(id, searchButtons);
            searchButton.jqxButton({ theme: 'energyblue' });
            searchButton.on('click', function () {
                var val = searchText.jqxInput('val');
                if (val) {
                    searchRooms(val)
                }
                else {
                    alert('请输入房间Id或房间名');
                }
            });

            var roomGroupTree = GetElement(id, roomGroupTrees);
            roomGroupTree.jqxTree({
                theme: 'energyblue', width: '100%', height: '100%',
                source: allTopLevelRoomGroups[index].items, toggleMode: 'click', toggleIndicatorSize: 16
            });

            var roomGroupDockPanel = GetElement(id, roomGroupDockPanels);
            roomGroupDockPanel.jqxDockPanel({ theme: 'energyblue', width: '100%', height: '100%', lastchildfill: true });

            var rankPanel = GetElement(id, rankPanels);
            rankPanel.html(originalRankPanel.html());
            var rankDockPanel = rankPanel.find('div.rankDockPanel');
            var rankTab = rankPanel.find('div.rankTab');
            rankDockPanels.push({ id: id, element: rankDockPanel });
            rankTabs.push({ id: id, element: rankTab });
            rankDockPanel.jqxDockPanel({ theme: 'energyblue', width: '100%', height: '100%', lastchildfill: true });
            rankTab.jqxTabs({ theme: 'energyblue', width: '100%', height: '100%', showCloseButtons: false, autoHeight: true });

            var roomGroupTree = GetElement(id, roomGroupTrees);
            roomGroupTree.on('select', function (e) {
                var args = e.args;
                var item = roomGroupTree.jqxTree('getItem', args.element);
                if (item) {
                    var rg = GetRoomGroup(item.value);
                    selectRoomGroup(rg);
                }
            });

            if (index == 0) {
                //window.external.WebPageContentLoaded();
            }
        }
    });
    roomGroupTab.on('selected', function (event) {
        currentTabIndex = event.args.item;
    });
    //roomGroupTab.attr('disabled', true);
}

function ReleaseMemory() {
    allTopLevelRoomGroups = null;
    searchResult = null;
    currentTabIndex = 0;
    roomGroupTab = null;
    roomGroupTabHeader = null;
    roomGroupTrees = null;
    rankSplitters = null;
    roomGroupSplitters = null;
    roomsPanels = null;
    rankPanels = null;
    searchTexts = null;
    searchButtons = null;
    locations = null;
    roomGroupTreeDockPanels = null;
    roomGroupDockPanels = null;
    rankPanels = null;
    rankDockPanels = null;
    rankTabs = null;
    originalRankPanel = null;
    //loginWindow = null;
    loginingWindow = null;
    userName = null;
    password = null;
    remember = null;
    autologin = null;
    loginButton = null;
}

function GetRoom(roomGroup, rId) {
    if (roomGroup.nodes) {
        for (var i = 0; i < roomGroup.nodes.length; i++) {
            if (roomGroup.nodes[i].id == rId) {
                return roomGroup.nodes[i];
            }
        }
    }
    if (roomGroup.items) {
        for (var j = 0; j < roomGroup.items.length; j++) {
            var room = GetRoom(roomGroup.items[j], rId);
            if (room) {
                return room;
            }
        }
    }
    return null;
}

function GetRoomGroup(id) {
    return GetRoomGroupInArray(allTopLevelRoomGroups, id);
}

function GetRoomGroupInArray(groups, id) {
    for (var i = 0; i < groups.length; i++) {
        if (groups[i].id == id) {
            return groups[i];
        }
        else if (groups[i].items) {
            var g = GetRoomGroupInArray(groups[i].items, id);
            if (g) {
                return g;
            }
        }
    }
    return null;
}

function Resize(w, h) {
}

function UpdateOnlineUserCountAsync(roomGroupsCount, roomsCount) {
    try {
        Concurrent.Thread.create(UpdateRoomGroupOnlineUserCount, roomGroupsCount);
    }
    catch (err) { }
    try {
        Concurrent.Thread.create(UpdateRoomOnlineUserCount, roomsCount);
    }
    catch (err) { }
}

function UpdateRoomGroupOnlineUserCount(roomGroupsCount) {
    if (roomGroupsCount) {
        var rgUserCounts = eval(roomGroupsCount);
        if (rgUserCounts && $.isArray(rgUserCounts) && rgUserCounts.length > 0) {
            for (var i = 0; i < rgUserCounts.length; i++) {
                try {
                    var roomGroup = GetRoomGroup(rgUserCounts[i].id);
                    var roomGroupTree = GetElement(rgUserCounts[i].rootid, roomGroupTrees);
                    var li = roomGroupTree.find("li:has(div:contains('" + roomGroup.label + "'))").last().get(0);
                    roomGroup.label = rgUserCounts[i].label;
                    roomGroup.count = rgUserCounts[i].count;
                    if (li) {
                        roomGroupTree.jqxTree('updateItem', li, { label: roomGroup.label, icon: roomGroup.icon });
                    }
                }
                catch (err) { }
            }
        }
    }
}

function UpdateRoomOnlineUserCount(roomsCount) {
    if (roomsCount) {
        var rUserCounts = eval(roomsCount);
        if (rUserCounts && $.isArray(rUserCounts) && rUserCounts.length > 0) {
            for (var i = 0; i < rUserCounts.length; i++) {
                try {
                    for (var j = 0; j < allTopLevelRoomGroups.length; j++) {
                        if (allTopLevelRoomGroups[j].id == rUserCounts[i].rootid) {
                            var room = GetRoom(allTopLevelRoomGroups[j], rUserCounts[i].id);
                            if (room) {
                                room.count = rUserCounts[i].count;
                                var roomsPanel = GetElement(rUserCounts[i].rootid, roomsPanels);
                                var roomDiv = roomsPanel.find("div[roomId='" + rUserCounts[i].id + "']").first();
                                var span = roomDiv.find('span:nth-child(3)').text("人数：" + room.count + "/" + room.maxcount);
                            }
                        }
                    }
                }
                catch (err) { }
            }
        }
    }
}
