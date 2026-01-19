using static Clone_KySoDienTu.SystemConstants.VanBanKiSo;

namespace Clone_KySoDienTu.Helpers
{
    public static class TenThaoTacHelper
    {
        public static string GetTenThaoTac(int maThaoTac)
        {
            string tenThaoTac = null;

            switch (maThaoTac)
            {
                case MA_THAO_TAC.HUY_KY:
                    tenThaoTac = TEN_THAO_TAC.HUY_KY;
                    break;
                case MA_THAO_TAC.GO_DUYET:
                    tenThaoTac = TEN_THAO_TAC.GO_DUYET;
                    break;
                case MA_THAO_TAC.YEU_CAU_HUY:
                    tenThaoTac = TEN_THAO_TAC.YEU_CAU_HUY;
                    break;
                case MA_THAO_TAC.TU_CHOI_HUY:
                    tenThaoTac = TEN_THAO_TAC.TU_CHOI_HUY;
                    break;
                case MA_THAO_TAC.GO_YEU_CAU_HUY:
                    tenThaoTac = TEN_THAO_TAC.GO_YEU_CAU_HUY;
                    break;
            }
            return tenThaoTac;
        }
    }
}