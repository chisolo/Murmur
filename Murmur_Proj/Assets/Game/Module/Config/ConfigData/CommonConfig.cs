using System.Collections.Generic;
using UnityEngine;

public class CommonConfig
{
    public Vector3 camera_pos;
    public Vector3 camera_boundary;
    public float camera_min_dis;
    public float camera_max_dis;
    public float camera_init_dis;
    public Vector3 food_take_pos;
    public Vector3 food_eat_pos;

    /// <summary>
    /// 初始雇员候选人刷新时间
    /// </summary>
    public int default_candidate_refresh_seconds;
    /// <summary>
    /// 初始雇员候选人数量
    /// </summary>
    public int default_candidate_count;
    /// <summary>
    /// 雇员候选人抽选池
    /// </summary>
    public List<string> candidate_gacha_groups;

    public int normal_base_salary = 10;
    public int rare_base_salary = 15;
    public int sr_base_salary = 20;

    /// <summary>
    /// 雇员最大数
    /// </summary>
    public int default_staff_max_counnt = 20;

    /// <summary>
    /// 雇员候选人刷新次数
    /// </summary>
    public int default_candidate_refresh_count = 5;

    /// <summary>
    /// 概率AlmostZero雇佣系数
    /// </summary>
    public float hire_probability_zero_ratio = 0.5f;
    /// <summary>
    /// 概率low雇佣系数
    /// </summary>
    public float hire_probability_low_ratio = 0.667f;
    /// <summary>
    /// 概率medium雇佣系数
    /// </summary>
    public float hire_probability_medium_ratio = 1f;
    /// <summary>
    /// 概率high雇佣系数
    /// </summary>
    public float hire_probability_high_ratio = 1f;
    /// <summary>
    /// 概率very high雇佣系数
    /// </summary>
    public float hire_probability_very_high_ratio = 1f;


    public float talk_duration = 3f;
    public float eat_duration = 3f;
    public int reception_worker_salary;
    public int sausage_storage_worker_salary;
    public int cheese_storage_worker_salary;
    public int flour_storage_worker_salary;
    public int deliver_worker_salary;
    public float default_puppet_speed = 2f;

    public int team_max = 4;
    public int ad_max = 5;

    public int basic_gacha_refresh_time = 10;

    // 离线收益跳转商店特权ID
    public string welcome_upgrade_vault_id;
    public int offline_time = 120;
    public double offline_income_factor = 0.00075;

    public float ad_income_ratio = 1.0f;
    public int ad_income_boost_limit_time = 60;
    public int ad_income_boost_time = 10;
    public string ad_income_upgrade_treasury_id;

    public string ad_extra_shop_gift_id;
    public string ad_extra_shop_limit_gift_id;
    public string ad_extra_require_build_id;
    public int ad_extra_button_cooldown = 20;
    public List<float> ad_extra_factor;

    public int no_ad_show_star_count;
    public int promotion_popup_show_star_count;

    public int star_trigger_rate = 5;

    public int show_promotion_by_popup_time = 20;

    public int income_boost_anim_unlock_star = 2;
    public int free_gacha_reddot_unlock_star = 2;
}
