public static class TalentEnhanecType
{
    public const string CAPACITY_DELIVER = "a_capacity_deliver_cell_value"; // 增加运输部门运输道具上限
    public const string EXTRA_REVENUE_HOTDOG = "a_extra_revenue_hotdog_reception_cell_value"; // 增加热狗客人前台接待额外收入概率
    public const string EXTRA_REVENUE_HAMBERG = "a_extra_revenue_hamberg_reception_cell_value"; // 增加汉堡客人前台接待额外收入概率
    public const string EXTRA_REVENUE_PIZZA = "a_extra_revenue_pizza_reception_cell_value"; // 增加披萨客人前台接待额外收入概率
    public const string REVENUE_HOTDOG = "a_revenue_hotdog_bureau_ratio"; // 提升热狗收入百分比
    public const string REVENUE_HAMBERG = "a_revenue_hamberg_bureau_ratio"; // 提升汉堡车收入百分比
    public const string REVENUE_PIZZA = "a_revenue_pizza_bureau_ratio"; // 提升披萨车收入百分比
    public const string LESS_SAUSAGE_HOTDOG = "s_sausage_hotdog_bureau_value"; // 减少热狗车所需香肠数量
    public const string LESS_FLOUR_HAMBERG = "s_flour_hamberg_bureau_value"; // 减少汉堡车所需面粉数量
    public const string LESS_SAUSAGE_HAMBERG = "s_sausage_hamberg_bureau_value"; // 减少汉堡车所需香肠数量
    public const string LESS_FLOUR_PIZZA = "s_flour_pizza_bureau_value"; // 减少披萨车所需面粉数量
    public const string LESS_CHEESE_PIZZA = "s_cheese_pizza_bureau_value"; // 减少披萨车所需奶酪数量
    public const string LESS_SAUSAGE_PIZZA = "s_sausage_pizza_bureau_value"; // 减少披萨车所需香肠数量
    public const string LESS_SALARY_HOTDOG = "s_salary_hotdog_bureau_ratio"; // 减少热狗车厨师薪水
    public const string LESS_SALARY_HAMBERG = "s_salary_hamberg_bureau_ratio"; // 减少汉堡车厨师薪水
    public const string LESS_SALARY_PIZZA = "s_salary_pizza_bureau_ratio"; // 减少披萨车厨师薪水
    public const string DELIVER_SPEED = "a_move_speed_deliver_cell_ratio"; // 增加运输部门运输速度
    public const string SERVICE_DURATION_HOTDOG = "d_service_duration_hotdog_bureau_ratio"; // 增加热狗车服务速度
    public const string SERVICE_DURATION_HAMBERG = "d_service_duration_hamberg_bureau_ratio"; // 增加汉堡车服务速度
    public const string SERVICE_DURATION_PIZZA = "d_service_duration_pizza_bureau_ratio"; // 增加披萨车服务速度
    public const string SERVICE_DURATION_RECEPTION = "d_service_duration_reception_cell_ratio"; // 增加接待台服务速度
    public const string LVLUP_COST_RECEPTION_CELL = "s_lvlup_cost_reception_cell_ratio"; // 接待台升级打折
    public const string LVLUP_COST_HOTDOG = "s_lvlup_cost_hotdog_bureau_ratio"; // 热狗车升级打折lv
    public const string LVLUP_COST_HAMBERG = "s_lvlup_cost_hamberg_bureau_ratio"; // 汉堡车升级打折
    public const string LVLUP_COST_PIZZA = "s_lvlup_cost_pizza_bureau_ratio"; // 披萨车升级打折
    public const string LVLUP_COST_DELIVER_CELL = "s_lvlup_cost_deliver_cell_ratio"; // 运输子房间升级打折
    public const string LVLUP_COST_SAUSAGE_CELL = "s_lvlup_cost_sausage_storage_cell_ratio"; // 香肠子房间升级打折
    public const string LVLUP_COST_FLOUR_CELL = "s_lvlup_cost_flour_storage_cell_ratio"; // 面粉子房间升级打折
    public const string LVLUP_COST_CHEESE_CELL = "s_lvlup_cost_cheese_storage_cell_ratio"; // 奶酪子房间升级打折
    public const string ENHANCE_LVLUP_DISCOUNT_HOTDOG = "a_enhance_lvlup_discount_hotdog_bureau_value"; // 热狗车强化打折
    public const string ENHANCE_LVLUP_DISCOUNT_HAMBERG = "a_enhance_lvlup_discount_hamberg_bureau_value"; // 汉堡车强化打折
    public const string ENHANCE_LVLUP_DISCOUNT_PIZZA = "a_enhance_lvlup_discount_pizza_bureau_value"; // 披萨车强化打折
    public const string ENHANCE_LVLUP_DISCOUNT_DELIVER_CELL = "a_enhance_lvlup_discount_deliver_cell_value"; // 运输子房间强化打折
    public const string ENHANCE_LVLUP_DISCOUNT_SAUSAGE_CELL = "a_enhance_lvlup_discount_sausage_storage_cell_value"; // 香肠子房间强化打折
    public const string ENHANCE_LVLUP_DISCOUNT_FLOUR_CELL = "a_enhance_lvlup_discount_flour_storage_cell_value"; // 面粉子房间强化打折
    public const string ENHANCE_LVLUP_DISCOUNT_CHEESE_CELL = "a_enhance_lvlup_discount_cheese_storage_cell_value"; // 奶酪子房间强化打折
    public const string GUEST_INTERVAL = "GUEST_INTERVAL"; // 增加顾客数量
    public const string CANDIDATE_NUMBER = "CANDIDATE_NUMBER"; // 增加雇佣界面候选人数
    public const string CANDIDATE_REFRESH_TIME = "CANDIDATE_REFRESH_TIME"; // 减少雇佣界面刷新时间
    public const string DISCOUNT_TALENT = "DISCOUNT_TALENT"; // 降低研究费用


    public static ITalentApply GetApplyTarget(string enhanceType)
    {
        switch (enhanceType) {
            case DISCOUNT_TALENT:
                return TalentModule.Instance;
            case CANDIDATE_NUMBER:
            case CANDIDATE_REFRESH_TIME:
                return TalentApplyMoudleSample.Instance;
            case CAPACITY_DELIVER:
            case EXTRA_REVENUE_HOTDOG:
            case EXTRA_REVENUE_HAMBERG:
            case EXTRA_REVENUE_PIZZA:
            case REVENUE_HOTDOG:
            case REVENUE_HAMBERG:
            case REVENUE_PIZZA:
            case LESS_SAUSAGE_HOTDOG:
            case LESS_FLOUR_HAMBERG:
            case LESS_SAUSAGE_HAMBERG:
            case LESS_FLOUR_PIZZA:
            case LESS_CHEESE_PIZZA:
            case LESS_SAUSAGE_PIZZA:
            case LESS_SALARY_HAMBERG:
            case LESS_SALARY_PIZZA:
            case LESS_SALARY_HOTDOG:
            case DELIVER_SPEED:
            case SERVICE_DURATION_HAMBERG:
            case SERVICE_DURATION_PIZZA:
            case SERVICE_DURATION_HOTDOG:
            case SERVICE_DURATION_RECEPTION:
            case LVLUP_COST_RECEPTION_CELL:
            case LVLUP_COST_HOTDOG:
            case LVLUP_COST_HAMBERG:
            case LVLUP_COST_PIZZA:
            case ENHANCE_LVLUP_DISCOUNT_HAMBERG:
            case ENHANCE_LVLUP_DISCOUNT_PIZZA:
            case ENHANCE_LVLUP_DISCOUNT_HOTDOG:
            case LVLUP_COST_FLOUR_CELL:
            case LVLUP_COST_DELIVER_CELL:
            case LVLUP_COST_CHEESE_CELL:
            case LVLUP_COST_SAUSAGE_CELL:
            case ENHANCE_LVLUP_DISCOUNT_FLOUR_CELL:
            case ENHANCE_LVLUP_DISCOUNT_DELIVER_CELL:
            case ENHANCE_LVLUP_DISCOUNT_CHEESE_CELL:
            case ENHANCE_LVLUP_DISCOUNT_SAUSAGE_CELL:
                return BuildingModule.Instance;
        }

        return null;
    }

}