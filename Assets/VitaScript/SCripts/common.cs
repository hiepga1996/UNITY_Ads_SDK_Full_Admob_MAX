using System;
using UnityEngine;

namespace Ultility
{
    public class common
    {
        /*----data key save----------------------------------------------------------------------------------------------------*/

        public static string GameKey = "Fruity_crush_";
        public static string REMOVE_ADS_KEY = GameKey + "REMOVE_ADS_KEY";


        public static string SaveDayReceiveSub = GameKey + "save_day_receive_sub";
        public static string SaveTimeEndDaylyVip = GameKey + "SaveTimeEndDaylyVip";

        public static string SaveTimeStartThreeDayVip = GameKey + "SaveTimeStartThreeDayVip";
        public static string SaveTimeEndThreeDayVip = GameKey + "SaveTimeEndThreeDayVip";

        public static string SaveTimeStartSevenDayVip = GameKey + "SaveTimeStartSevenDayVip";
        public static string SaveTimeEndSevenDayVip = GameKey + "SaveTimeEndSevenDayVip";

        public static string SaveTimeStartThirtyDayVip = GameKey + "SaveTimeStartThirtyDayVip";
        public static string SaveTimeEndThirtyDayVip = GameKey + "SaveTimeEndThirtyDayVip";

        public static string SaveLevelNotAds = GameKey + "SaveLevelNotAds";
        public static string SaveTicketSpinDayly = GameKey + "SaveTicketSpinDayly";

        public static string SaveNumberTurnMoreDay = GameKey + "SaveNumberTurnMoreDay";
        public static string SaveNumberTurnMoreThree = GameKey + "SaveNumberTurnMoreThree";
        public static string SaveNumberTurnMoreSeven = GameKey + "SaveNumberTurnMoreSeven";
        public static string SaveNumberTurnMoreThirty = GameKey + "SaveNumberTurnMoreThirty";

        public static bool isDailyRewardClaimed = false;
        public static bool loadLevelConfigDone = false;
        public static bool loadScoreConfigDone = false;

        public static bool isStartGame = false;

        #region Data
        public static bool showRateInTurn = false;
        public static bool is_open_game = false;
        //public static int config_design_event = 0;
        public static int level_more_not_Ads = 0;

        public static int num_turn_more_day = 5;
        public static int num_turn_more_three = 10;
        public static int num_turn_more_seven = 15;
        public static int num_turn_more_thirty = 30;

        public static bool level_have_increase_turn = false;
        public static bool can_touch_setting_ingame = true;
        public static int ticket_Spin_dayly = 0;
        public static bool is_received_sub = true;
        public static bool hide_booster_candy_boom = true;
        public static int styleBooster = 0;

        public static int num_vip_hammer = 0;
        public static int num_vip_candy_pack = 0;
        public static int num_vip_vboom = 0;
        public static int num_vip_hboom = 0;
        public static int num_vip_shuffle = 0;

        public static bool is_dayly_vip = false;
        public static long time_to_end_dayly_vip = 0;
        public static bool is_three_day_vip = false;
        public static long time_to_end_three_day_vip = 0;
        public static long time_start_three_day_vip = 0;
        public static bool is_seven_day_vip = false;
        public static long time_to_end_seven_day_vip = 0;
        public static long time_start_seven_day_vip = 0;
        public static bool is_thirty_day_vip = false;
        public static long time_to_end_thirty_day_vip = 0;
        public static long time_start_thirty_day_vip = 0;

        public static long time_get_url = 0;
        public static float countTimePlay = 0;
        public static int noti_capping_time = 0;
        public static int request_permission_notify = 0;
        public static string REQUEST_PERMISSION_NOTIFY = GameKey + "REQUEST_PERMISSION_NOTIFY";
        public static float timeCappingInterstitalInGame = 5*60;
        public static float countTimeInterstitalIngame = 0;
        public static bool is_tablet = false;
        public static bool isFold = false;

        #endregion

        #region Health System

        public static int health_count
        {
            get => PlayerPrefs.GetInt("health_count", 5);
            set
            {
                int addOrMinus = health_count + value;
                PlayerPrefs.SetInt("health_count", value);
                OnHealthChange?.Invoke(value, addOrMinus);
            }
        }
        public static event Action<long, long> OnHealthChange;

        #endregion

        #region Remote Config

        //public static int health_count = 5;

        //Remote Config
        public static bool canShowInterstitial5MinuteInGame = false;
        public static bool aoa_admob_on_off = false;
        public static bool resume_ads_on_off = false;
        public static bool ads_aoa_show = true;
        public static bool Mrec_Setting_On_Off = false;
        public static bool Mrec_Dayly_On_Off = false;
        public static bool Mrec_RewardItem_On_Off = false;
        public static bool mrec_coin_plus_on_off = false;
        public static bool mrec_challenge_on_off = false;
        public static bool mrec_game_over_on_off = false;
        public static bool mrec_coin_effect_on_off = false;
        public static bool mrec_loading_on_off = false;
        public static bool native_level_start_on_off = false;
        public static bool native_spin_on_off = false;
        public static bool native_game_play_on_off = false;
        public static bool native_shop_on_off = false;
        public static bool native_level_select_on_off = false;
        public static bool banner_on_off = false;
        public static bool switch_banner_when_false = false;
        public static bool first_time_spin = false;
        public static bool native_on_off = false;

        public static int ads_inter_type = 2;
        public static int ads_banner_type = 1;
        public static int ads_reward_type = 2;
        public static int multiple_reward_win = 2;

        public static bool percent_on_off = false;
        public static float percent_number = 0;
        public static float number_to_compair = 0;

        public static int time_loading_native = 3;

        public static int coin_reward = 10;
        public static int coin_reward_win = 10;
        public static int coin_reward_noti = 100;

        //public static string admob_native_ids_ads = "ca-app-pub-3260384659102457/1702283263";
        //public static string admob_banner_ids_ads = "ca-app-pub-3260384659102457/4702883522";
        public static string icon_game = "https://cdn.jsdelivr.net/gh/toannbhut/video@main/Layer%203.png";
        public static string link_game = "https://play.dragonland.uk";

        public static int ads_aoa_open_count = 1;
        public static bool banner_admob_on_off = true;
        public static bool banner_adaptive_on_off = true;
        public static bool limit_level_tree_show = true;

        public static bool ad_break_on_off = false;
        public static int ad_break_capping_time = 200;
        public static int win_game_coin_reward = 10;
        public static int timeCountDown = 300;
        public static int level_limit_fix = 300;
        public static int score_limit_fix = 100;

        public static int banner_show_level = 3;
        public static int AOA_show_level = 6;
        public static int mrec_show_level = 21;

        #endregion

        #region IAP
        public static string Coin1 = "coin1";
        public static string Coin2 = "coin2";
        public static string Coin3 = "coin3";
        public static string Coin4 = "coin4";
        public static string Coin5 = "coin5";
        public static string Removeads = "removeads";
        public static string DaylyVip = "daylyvip";
        public static string ThreeDayVip = "threedayvip";
        public static string SevenDayVip = "sevenDayVip";
        public static string ThirtyDayVip = "thirtydayvip";
        public static string CoinAndItem1 = "coin_item_1";
        public static string CoinAndItem2 = "coin_item_2";
        public static string CoinAndItem3 = "coin_item_3";
        public static string CoinAndItem4 = "coin_item_4";
        public static string CoinAndItem5 = "coin_item_5";
        #endregion

        public static string FormatMoney(float value)
        {
            var x1 = 3;
            var newstr = "";
            var count = 0;

            var p = value.ToString().Split('.');
            var cF = p[0];
            char[] ch = new char[cF.Length];
            for (int i = 0; i < cF.Length; i++)
            {
                count++;
                if (count % x1 == 1 && count != 1)
                {
                    newstr = newstr + ',' + cF[i];
                }
                else
                {
                    newstr = newstr + cF[i];
                }
            }

            //Debug.Log("newstr = " + newstr);
            return newstr;

        }

        public static string FormatMoneyK(float value)
        {
            var newstr = "";
            if (value < 1000)
            {
                newstr = value.ToString();
            }
            else if (value >= 1000 && value < 1000000)
            {
                var newvalue = (value / 1000).ToString("N2");

                if (newvalue.Contains('.'))
                {
                    var p = newvalue.Split('.');
                    var cF = p[0];
                    newstr = FormatMoney(float.Parse(cF)) + "," + p[1] + "K";
                }
                else
                {
                    var p = newvalue.Split(',');
                    var cF = p[0];
                    newstr = FormatMoney(float.Parse(cF)) + "," + p[1] + "K";
                }
            }
            else if (value >= 1000000 && value < 1000000000)
            {
                var newvalue = (value / 1000000).ToString("N2");
                if (newvalue.Contains('.'))
                {
                    var p = newvalue.Split('.');
                    var cF = p[0];
                    newstr = FormatMoney(float.Parse(cF)) + "," + p[1] + "M";
                }
                else
                {
                    var p = newvalue.Split(',');
                    var cF = p[0];
                    newstr = FormatMoney(float.Parse(cF)) + "," + p[1] + "M";
                }
            }
            else
            {
                var newvalue = (value / 1000000000).ToString("N2");
                if (newvalue.Contains('.'))
                {
                    var p = newvalue.Split('.');
                    var cF = p[0];
                    newstr = FormatMoney(float.Parse(cF)) + "," + p[1] + "B";
                }
                else
                {
                    var p = newvalue.Split(',');
                    var cF = p[0];
                    newstr = FormatMoney(float.Parse(cF)) + "," + p[1] + "B";
                }
            }

                return newstr;
        }
    }
}

