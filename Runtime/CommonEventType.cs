﻿public enum CommonEventType
{
    SOCKET_LOGIN_SUCCESSFUL,
    SOCKET_LOGIN_FAILED,
    SOCKET_CONTENTED,
    SOCKET_DIS_CONTENTED,

    EXIT_OTHERGAME,
    EXIT_EDITOR,
    RESET_WOOGI,

    LOAD_SCENE_ASYNC_START,
    LOAD_SCENE_ASYNC_DONE,
    UPDATA_WOOGI_ATTRIBUTE,

    //keyboard input
    ANYKEY_INPUT,
    SWITCH_KEYBOARD,
    OPENKEYBOARD_INPUT_ENTER,
    CLOSEKEYBOARD_INPUT_ENTER,
    CHAT_NEW_LOCAL_MESSAGE,
    ONRECIVECHATMESSAGES,

    //Language
    LOADLANGUAGE_READY,

    Updata_Coins_Number,
    ENTER_HOMEWORLD_STATE,
    BACKPACK_OPEN_CLOSE,
    UNLOAD_PROP,
    SET_CURSOR_DRAG_STATE,
    SHOW_BACKPACK_PANEL,
    BACKPACK_CHOOSE_IMAGE,
    PROPBACKPACKMENU_POWER_CHANGE_STATE,
    PROPBACKPACKMENU_NOTIFY_EXCHANGE,
    PROPBACKPACKMENU_NOTIFY_CHANGE,
    PROPBACKPACKMENU_DRAG_ITEM_ISTOOL,
    PROPBACKPACKMENU_DRAG_ITEM,
    PROPBACKPACKMENU_SELECT_STATE,
    PROPBACKPACKMENU_TOP_STATE,
    PROPBACKPACKMENU_CONTENT_STATE,
    PROPBACKPACKMENU_CHANGE_STATE,
    PROPBACKPACKMENU_USER_ITEM_STATE,
    ENTER_SCENE_SUCCEED,
    PROPBACKPACKMENU_USER_STATE,
    PROP_BACKPACK_DRAG,
    GET_NET_PROP,
    UPDATE_POPUP_BG,
    PROP_CD_CREATE,
    PROPBACKPACKMENU_GRID_STATE,
    PROPBACKPACKMENUNEW_POWER_CHANGE_STATE,
    PROPBACKPACKMEUNNEW_UPDATE_GRID,
    PROPBACKPACKMEUNNEW_EXCHANGE_GRID,
    BACKPACK_SELF_FASHION,
    RES_SERVER_PING_FINISHED,
    CMD_RELOAD_UPDATE_CONFIG,
    CMD_CLEAR_UPDATE_CONFIG,
    MAIN_CHILD_MODULE_CHANGE,
    SWITCH_SCENE_START,
    SWITCH_SCENE_END,
    //profile
    CLOSE_PlayerProfilePanle,
    MAINPLANE_INIT_OVER,
    UPDATE_SKILLS,
    SEASON_XP,
    POPUP_CLOSED,
    NEW_LOADING_WINDOW,
    LANGUANGE_CHANGE,
    //news
    DISCONNECTED,
    MINEELEVATOR_LOADING_OVER,
    POPUP_Load,
    //radar
    RADAR_TARGET_ENABLE,
    RADAR_TARGET_DISABLE,
    MODIFY_3DUI_NAMELABLE_CAMERA,
    //chat
    CHAT_TEAM_GROUP_MESSAGE,
    CHAT_MESSAGE_TIPS,
    CHAT_TEAM_GROUP_MESSAGE_OVER,
    CHAT_NEW_CITY_MESSAGE,
    CHAT__TEAM_MESSAGE_TIPS,
    CHAT_TEAM_NEW_GROUP_MESSAGE,
    CHAT_TEAM_HISTORY_GROUP_MESSAGE,
    ChAT_NEW_TEAM_MESSAGE_TO_3DWOOGI,
    CHAT_NEW_CITY_MESSAGE_TO_3DWOOGI,
    UPDATEPHONE_UNREAD,
    UPDATEPHONE_UI,
    CHAT_NEW_MESSAGE_TO_3DWOOGI,
    CHAT_NEW_PRIVATE_MESSAGE,
    CHAT_READ_TYPE,
    CITZEN_LIST_DATA_UPDATE,
    CITZEN_SET_ONLINE,
    ENTER_VEHICLECHECK,
    WOOGIPLAYER_ACTION_STATE,
    MAINUI_RESETBTN_CD,
    SHOW_USER_INFOM,
    SetNetProp,
    MODEL_FOG_OFF,
    MODEL_FOG_ON,
    UPDATE_NEARESTCITY,
    UPDATE_MAPCACHE_SETTING,
    W_3DReset,
    CLEAR_MAP_CACHE,
    UPDATE_VIEW_MAX_DISTANCE,
    UPDATE_VIEW_DISTANCE,
    LOG_OUT,
    FIRST_ENTER_LOG_IN,
    PIRATECAVE_CREATE_PIRATE,
    PIRATECAVE_REMOVE_PIRATE_OBJ,
    PIRATE_CREATE_IN_WORLD,
    PIRATE_REMOVE_IN_WORLD,
    ATTACK,
    PROP_CD_COUNT,
    PAYMENT_FINISHED,
    //friend
    NEED_CLOSE_PHONE_UI,
    KEYBOARD_INPUT_ENTER,
    OPEN_CHATVIEW,
    NETWORK_GET_OBJECT_POS,
    CHAT_TOGGLE_STATE,
}
public enum ReceiptInfo
{
    no = 0,
    level = 1,
    video,
    vehicles,
    coins,
    game
}
public enum WcUserQuickBar
{
    getQuickBar,        //获取初始化数据
    initialQuickBar,    //添加和删除
    exchangeQuickBar,   //格子间的
}
public enum EventEnum
{
    UITextTool,
}
public enum TaskEventType
{
    TASK11_DOING,
    TASK11_CLICK_REST,
    TASK11_DRAG_BACKPACK_ITEM,
    TASK11_CLICK_BACKPACK,
    TASK11_CLICK_BACKPACK_TAB,
}
public enum PlayerActionState
{
    walking,
    running,
    move,
    driving,
    parachute,
    swimming
}
