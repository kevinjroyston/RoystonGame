﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RoystonGame.TV.GameModes.BriansGames.BodyBuilder.DataModels.Setup_Person;

namespace RoystonGame.TV.GameModes.BriansGames.BodyBuilder
{
    public class BodyBuilderConstants
    {
        public static Dictionary<DrawingType, int> widths = new Dictionary<DrawingType, int>()
        {
            {DrawingType.Head, 240},
            {DrawingType.Body, 240},
            {DrawingType.Legs, 240},
        };
        public static Dictionary<DrawingType, int> heights = new Dictionary<DrawingType, int>()
        {
            {DrawingType.Head, 120},
            {DrawingType.Body, 240},
            {DrawingType.Legs, 240},
        };
        public static Dictionary<DrawingType, string> backgrounds = new Dictionary<DrawingType, string>()
        {
            #region HeadBG
             {DrawingType.Head, "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAPAAAAB4CAYAAADMtn8nAAACHUlEQVR4nO3ZwWoTYRSA0ZumIkU0BTe+/4O5FMFNI0IraTpltItmFzfGT85Zziz+y9x8DEk2y7J8npnbAWru1oAfZuat1UHOzzXg48xc2R3kPK3h7u0NkvbevBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ9i15f19x8eZx/uZ4+H06O2bmeubme0/upXq3P8zj/wC1gi+f52538/M8nL+ZuZmN/Ph08z2vbk5j4AvYH2DPexnfnw7DWEzM+8+mpvzCfgC1s/+sswsT6ch/Lpmbv6AgC9gs5m52v7+7vg6hPXaes/cnMuv0BAmYAgTMIQJGMIEDGFrwDsLhKTdGvDB7iDpsP4P/GVmbu0PYmbungEVxFF+WFuqEgAAAABJRU5ErkJggg=="},
            #endregion

            #region BodyBG
            {DrawingType.Body, "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAPAAAADwCAYAAAA+VemSAAAEKUlEQVR4nO3a22ocRxRA0dMamUQYWwl+8f9/XDAYksgxyEGXDi0PiSYgUF4y2pq1HksPOkzV1qWrl3Vdf52Zn4f/zZ9/zHz5NHN9NTPr/rsuMxeXM+8/zvzw7mXuRXXuV+y381P/BI7h7mbm29XM18+HISwz8/aDuXk+AR/BdvbXdWa9PwzhYc3c/AcCPoJlmTnbzezeHIawrW1fMzfPdeaTgi4BQ5iAIUzAECZgCPMU+gi2a5f7u+/3qo+f5m5r6wu+j6nO/ZoJ+AiW/ZXMcvavFyL2L0WYm+cS8BFs96g/Xu4beBTCtvZwx2punsm70Edwdztze73/U/SRLYLzi5ndC/2xWp37FfMu9DFsB30XfPG/Ovdr5ik0hAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ9j5KW/e3e3M7fXM3c3h+u7NzPnFzO6kP52Xwz497aSP6HYovnyaub6amXW/uMxcXM68/zize3fkAXlgn5522r+Bb2a+Xc18/Xx4MJaZefvhyMPxN/v0tJMOeDsL6zqz3h8ejIe1I8/GP+zT00464GWZOdt9/1/q8cHY1rav8TLYp6d5Cg1hAoYwAUOYgCFMwBC2BXxpAyHpcgv4xt5B0s12D/zLzPxk/yBm5ve/AINppA0rSIxSAAAAAElFTkSuQmCC"},
            #endregion

            #region LegsBG
            {DrawingType.Legs, "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAPAAAADwCAYAAAA+VemSAAADhElEQVR4nO3aTYvTUBSA4ZNORUT8ws38/x8ngqCOCiqljVQGYRZdqn0nz7PM6ibnvuTSdFnX9Tgzu9mgn19nvryf+X43M+v9/S8zz17NvLydefpii0/l+pjTRaf9VuM9Ox5mftzNfPvwcGMsM/P87X9eHH+Y00W7/ZUu7J8474V1nVlPDzfG72uP/N5LzOmy/f0jWa51gX/TsszsbmZunjzcGOdryyafyHUyp4vWzR6f4TEQMIQJGMIEDGEChrDdVn+Bhkdg8QaGsPN34NNWj9K+LzaY00Wn83+hP87MmytdIHDZJ0doCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQJmAIEzCECRjCBAxhAoYwAUOYgCFMwBAmYAgTMIQJGMIEDGEChjABQ5iAIUzAECZgCBMwhAkYwgQMYQKGMAFDmIAhTMAQtqzrehQyJJ3O4R7MDpIO+5l5NzOvzQ9iZj7/AsBRSqqzpLyAAAAAAElFTkSuQmCC"}
            #endregion
        };
    }
}
