/*
    Re-Volt Track Editor - Unity Edition
    A version of the track editor re-built from the ground up in Unity
    Copyright (C) 2022 Dummiesman

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace Modules
{
	public enum Direction
    {
		North = 0,
		East = 1,
		South = 2,
		West = 3
    }

	public enum ID 
	{
		TWM_SPACE, //space_unit
		TWM_START, //start_grid
		TWM_SPACE_B, //space_unit
		TWM_BANK_05, //b_05
		TWM_BANK_10, //b_10
		TWM_BANK_20, //b_20
		TWM_BANK_05_2, //b_05_2
		TWM_BANK_10_2, //b_10_2
		TWM_BANK_20_2, //b_20_2
		TWM_BANK_05_2_LH, //b_05_2_LH
		TWM_BANK_10_2_LH, //b_10_2_LH
		TWM_BANK_20_2_LH, //b_20_2_LH
		TWM_BANK_05_2_N, //b_05_2_N
		TWM_BANK_10_2_N, //b_10_2_N
		TWM_BANK_20_2_N, //b_20_2_N
		TWM_BANK_CORNER_05, //bc_05
		TWM_BANK_CORNER_10, //bc_10
		TWM_BANK_CORNER_20, //bc_20
		TWM_BANK_CORNER_05_2, //bc_05_2
		TWM_BANK_CORNER_10_2, //bc_10_2
		TWM_BANK_CORNER_20_2, //bc_20_2
		TWM_BANK_IN_05, //bino_05
		TWM_BANK_IN_10, //bino_10
		TWM_BANK_IN_20, //bino_20
		TWM_BANK_IN_05_2, //bino_05_2
		TWM_BANK_IN_10_2, //bino_10_2
		TWM_BANK_IN_20_2, //bino_20_2
		TWM_BANK_IN_05_LH, //bino_05_LH
		TWM_BANK_IN_10_LH, //bino_10_LH
		TWM_BANK_IN_20_LH, //bino_20_LH
		TWM_BANK_IN_05_2_LH, //bino_05_2_LH
		TWM_BANK_IN_10_2_LH, //bino_10_2_LH
		TWM_BANK_IN_20_2_LH, //bino_20_2_LH
		TWM_BANK_IN_05_2_D, //bino_05_2_D
		TWM_BANK_IN_10_2_D, //bino_10_2_D
		TWM_BANK_IN_20_2_D, //bino_20_2_D
		TWM_BANK_IN_05_2_LH_D, //bino_05_2_LH_D
		TWM_BANK_IN_10_2_LH_D, //bino_10_2_LH_D
		TWM_BANK_IN_20_2_LH_D, //bino_20_2_LH_D
		TWM_BANK_IN_05_2_N, //bino_05_2_N
		TWM_BANK_IN_10_2_N, //bino_10_2_N
		TWM_BANK_IN_20_2_N, //bino_20_2_N
		TWM_BANK_IN_05_2_LH_N, //bino_05_2_LH_N
		TWM_BANK_IN_10_2_LH_N, //bino_10_2_LH_N
		TWM_BANK_IN_20_2_LH_N, //bino_20_2_LH_N
		TWM_BRIDGE_10, //bridge_10
		TWM_BRIDGE_15, //bridge_15
		TWM_BRIDGE_20, //bridge_20
		TWM_BRIDGE_25, //bridge_25
		TWM_BRIDGE_30, //bridge_30
		TWM_BRIDGE_35, //bridge_35
		TWM_BRIDGE_40, //bridge_40
		TWM_BRIDGE_45, //bridge_45
		TWM_BRIDGE_50, //bridge_50
		TWM_BRIDGE_55, //bridge_55
		TWM_BRIDGE_60, //bridge_60
		TWM_BRIDGE_65, //bridge_65
		TWM_BRIDGE_70, //bridge_70
		TWM_BRIDGE_75, //bridge_75
		TWM_BRIDGE_80, //bridge_80
		TWM_BRIDGE_10_2, //bridge_10_2
		TWM_BRIDGE_15_2, //bridge_15_2
		TWM_BRIDGE_20_2, //bridge_20_2
		TWM_BRIDGE_25_2, //bridge_25_2
		TWM_BRIDGE_30_2, //bridge_30_2
		TWM_BRIDGE_35_2, //bridge_35_2
		TWM_BRIDGE_40_2, //bridge_40_2
		TWM_BRIDGE_45_2, //bridge_45_2
		TWM_BRIDGE_50_2, //bridge_50_2
		TWM_BRIDGE_55_2, //bridge_55_2
		TWM_BRIDGE_60_2, //bridge_60_2
		TWM_BRIDGE_65_2, //bridge_65_2
		TWM_BRIDGE_70_2, //bridge_70_2
		TWM_BRIDGE_75_2, //bridge_75_2
		TWM_BRIDGE_80_2, //bridge_80_2
		TWM_BRIDGE_10_2_LH, //bridge_10_2_LH
		TWM_BRIDGE_15_2_LH, //bridge_15_2_LH
		TWM_BRIDGE_20_2_LH, //bridge_20_2_LH
		TWM_BRIDGE_25_2_LH, //bridge_25_2_LH
		TWM_BRIDGE_30_2_LH, //bridge_30_2_LH
		TWM_BRIDGE_35_2_LH, //bridge_35_2_LH
		TWM_BRIDGE_40_2_LH, //bridge_40_2_LH
		TWM_BRIDGE_45_2_LH, //bridge_45_2_LH
		TWM_BRIDGE_50_2_LH, //bridge_50_2_LH
		TWM_BRIDGE_55_2_LH, //bridge_55_2_LH
		TWM_BRIDGE_60_2_LH, //bridge_60_2_LH
		TWM_BRIDGE_65_2_LH, //bridge_65_2_LH
		TWM_BRIDGE_70_2_LH, //bridge_70_2_LH
		TWM_BRIDGE_75_2_LH, //bridge_75_2_LH
		TWM_BRIDGE_80_2_LH, //bridge_80_2_LH
		TWM_BRIDGE_10_2_N, //bridge_10_2_N
		TWM_BRIDGE_15_2_N, //bridge_15_2_N
		TWM_BRIDGE_20_2_N, //bridge_20_2_N
		TWM_BRIDGE_25_2_N, //bridge_25_2_N
		TWM_BRIDGE_30_2_N, //bridge_30_2_N
		TWM_BRIDGE_35_2_N, //bridge_35_2_N
		TWM_BRIDGE_40_2_N, //bridge_40_2_N
		TWM_BRIDGE_45_2_N, //bridge_45_2_N
		TWM_BRIDGE_50_2_N, //bridge_50_2_N
		TWM_BRIDGE_55_2_N, //bridge_55_2_N
		TWM_BRIDGE_60_2_N, //bridge_60_2_N
		TWM_BRIDGE_65_2_N, //bridge_65_2_N
		TWM_BRIDGE_70_2_N, //bridge_70_2_N
		TWM_BRIDGE_75_2_N, //bridge_75_2_N
		TWM_BRIDGE_80_2_N, //bridge_80_2_N
		TWM_CORNER_SQUARE, //c_basic
		TWM_CORNER_SQUARE_2, //c_basic_2
		TWM_CORNER_REGULAR, //c_l_r
		TWM_CORNER_REGULAR_2, //c_l_r_2
		TWM_CORNER_REGULAR_LH, //c_l_r_LH
		TWM_CORNER_REGULAR_2_LH, //c_l_r_2_LH
		TWM_CORNER_REGULAR_N, //c_l_r_N
		TWM_CORNER_REGULAR_2_N, //c_l_r_2_N
		TWM_CORNER_MEDIUM, //c_l_x
		TWM_CORNER_MEDIUM_2, //c_l_x_2
		TWM_CORNER_MEDIUM_LH, //c_l_x_LH
		TWM_CORNER_MEDIUM_2_LH, //c_l_x_2_LH
		TWM_CORNER_MEDIUM_N, //c_l_x_N
		TWM_CORNER_MEDIUM_2_N, //c_l_x_2_N
		TWM_CORNER_LARGE, //c_l_xt
		TWM_CORNER_LARGE_2, //c_l_xt_2
		TWM_CORNER_LARGE_LH, //c_l_xt_LH
		TWM_CORNER_LARGE_2_LH, //c_l_xt_2_LH
		TWM_CORNER_LARGE_N, //c_l_xt_N
		TWM_CORNER_LARGE_2_N, //c_l_xt_2_N
		TWM_DIP_R, //d_r
		TWM_DIP_X, //d_x
		TWM_DIP_XT, //d_xt
		TWM_DIP_R_2, //d_r_2
		TWM_DIP_X_2, //d_x_2
		TWM_DIP_XT_2, //d_xt_2
		TWM_HUMP_R, //h_r
		TWM_HUMP_X, //h_x
		TWM_HUMP_XT, //h_xt
		TWM_HUMP_R_2, //h_r_2
		TWM_HUMP_X_2, //h_x_2
		TWM_HUMP_XT_2, //h_xt_2
		TWM_NARROW, //narrow
		TWM_NARROWF, //narrowf
		TWM_NARROWF_2, //narrowf_2
		TWM_PIPE_0, //p_(0)
		TWM_PIPE_1, //p_(1)
		TWM_PIPE_2, //p_(2)
		TWM_PIPE_20_0, //p_20_(0)
		TWM_PIPE_20_1_BOT, //p_20_(1,bot)
		TWM_PIPE_20_1_TOP, //p_20_(1,top)
		TWM_PIPE_20_2, //p_20_(2)
		TWM_PIPEC_0, //pc_(0)
		TWM_PIPEC_1, //pc_(1)
		TWM_PIPEC_1A, //pc_(1a)
		TWM_PIPEC_2, //pc_(2)
		TWM_PIPEF, //pf
		TWM_PIPE_1A, //p_(1a)
		TWM_PIPEF_OUT, //pf_out
		TWM_PIPEF_N, //pf_n
		TWM_RAMP_00, //r_00
		TWM_RAMP_05, //r_05
		TWM_RAMP_10, //r_10
		TWM_RAMP_15, //r_15
		TWM_RAMP_20, //r_20
		TWM_RAMP_25, //r_25
		TWM_RAMP_30, //r_30
		TWM_RAMP_00_2, //r_00_2
		TWM_RAMP_05_2, //r_05_2
		TWM_RAMP_10_2, //r_10_2
		TWM_RAMP_15_2, //r_15_2
		TWM_RAMP_20_2, //r_20_2
		TWM_RAMP_25_2, //r_25_2
		TWM_RAMP_30_2, //r_30_2
		TWM_RAMP_05_2_D, //r_05_2_D
		TWM_RAMP_10_2_D, //r_10_2_D
		TWM_RAMP_15_2_D, //r_15_2_D
		TWM_RAMP_20_2_D, //r_20_2_D
		TWM_RAMP_25_2_D, //r_25_2_D
		TWM_RAMP_30_2_D, //r_30_2_D
		TWM_RAMP_05_2_N, //r_05_2_N
		TWM_RAMP_10_2_N, //r_10_2_N
		TWM_RAMP_15_2_N, //r_15_2_N
		TWM_RAMP_20_2_N, //r_20_2_N
		TWM_RAMP_25_2_N, //r_25_2_N
		TWM_RAMP_30_2_N, //r_30_2_N
		TWM_DIAGONAL_01, //sf_01
		TWM_DIAGONAL_02, //sf_02
		TWM_DIAGONAL_01_2, //sf_01_2
		TWM_DIAGONAL_02_2, //sf_02_2
		TWM_DIAGONAL_01_LH, //sf_01_LH
		TWM_DIAGONAL_02_LH, //sf_02_LH
		TWM_DIAGONAL_01_2_LH, //sf_01_2_LH
		TWM_DIAGONAL_02_2_LH, //sf_02_2_LH
		TWM_RUMBLE_02, //st_02
		TWM_RUMBLE_04, //st_04
		TWM_RUMBLE_02_2, //st_02_2
		TWM_RUMBLE_04_2, //st_04_2
		TWM_CROSS_ROAD, //X
		TWM_ZIGZAG, //zigzag
		TWM_ZIGZAG_2, //zigzag_2
		TWM_JUMP,     //jump unit
		TWM_RAMP_00_2_N, //r_00_2_n
	}

	public class Lookup
    {
		public static readonly ModuleGroup[] Groups = new[]
		{
	       new ModuleGroup(LocString.MODULE_STARTGRID,   ID.TWM_START,            ID.TWM_START ),
	       new ModuleGroup(LocString.MODULE_STRAIGHTS,   ID.TWM_RAMP_00,          ID.TWM_RAMP_00_2_N),
						   
		   new ModuleGroup(LocString.MODULE_DIPS,		 ID.TWM_DIP_X,            ID.TWM_DIP_X_2),
	       new ModuleGroup(LocString.MODULE_HUMPS,       ID.TWM_HUMP_X,           ID.TWM_HUMP_X_2),
	       new ModuleGroup(LocString.MODULE_SQUARE_BEND, ID.TWM_CORNER_SQUARE,    ID.TWM_CORNER_SQUARE_2),
	       new ModuleGroup(LocString.MODULE_ROUND_BEND,  ID.TWM_CORNER_REGULAR_N, ID.TWM_CORNER_REGULAR_2_N),
	       new ModuleGroup(LocString.MODULE_DIAGONAL,    ID.TWM_DIAGONAL_02,      ID.TWM_DIAGONAL_02_2),
	       new ModuleGroup(LocString.MODULE_BANK,        ID.TWM_BANK_05,          ID.TWM_BANK_05_2_N),
	       new ModuleGroup(LocString.MODULE_RUMBLE,      ID.TWM_RUMBLE_02,        ID.TWM_RUMBLE_02_2),
	       new ModuleGroup(LocString.MODULE_NARROW,      ID.TWM_NARROW,           ID.TWM_NARROW),
						   
		   new ModuleGroup(LocString.MODULE_PIPE,        ID.TWM_PIPE_2,           ID.TWM_PIPE_2),
	       new ModuleGroup(LocString.MODULE_BRIDGE,      ID.TWM_BRIDGE_25,        ID.TWM_BRIDGE_25_2_N),
	       new ModuleGroup(LocString.MODULE_CROSSROAD,   ID.TWM_CROSS_ROAD,       ID.TWM_CROSS_ROAD),
	       new ModuleGroup(LocString.MODULE_JUMP,        ID.TWM_JUMP,             ID.TWM_JUMP),
	       new ModuleGroup(LocString.MODULE_CHICANE,     ID.TWM_ZIGZAG,           ID.TWM_ZIGZAG_2)
		};

		public static ReVolt.Track.AINodePriority[] ModulePriority = new[]
		{
			ReVolt.Track.AINodePriority.RacingLine, //space_unit
			ReVolt.Track.AINodePriority.RacingLine, //start_grid
			ReVolt.Track.AINodePriority.RacingLine, //space_unit
			ReVolt.Track.AINodePriority.RacingLine, //b_05
			ReVolt.Track.AINodePriority.RacingLine, //b_10
			ReVolt.Track.AINodePriority.RacingLine, //b_20
			ReVolt.Track.AINodePriority.RacingLine, //b_05_2
			ReVolt.Track.AINodePriority.RacingLine, //b_10_2
			ReVolt.Track.AINodePriority.RacingLine, //b_20_2
			ReVolt.Track.AINodePriority.RacingLine, //b_05_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //b_10_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //b_20_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //b_05_2_N
			ReVolt.Track.AINodePriority.RacingLine, //b_10_2_N
			ReVolt.Track.AINodePriority.RacingLine, //b_20_2_N
			ReVolt.Track.AINodePriority.Slowdown20, //bc_05
			ReVolt.Track.AINodePriority.Slowdown20, //bc_10
			ReVolt.Track.AINodePriority.Slowdown20, //bc_20
			ReVolt.Track.AINodePriority.Slowdown20, //bc_05_2
			ReVolt.Track.AINodePriority.Slowdown20, //bc_10_2
			ReVolt.Track.AINodePriority.Slowdown20, //bc_20_2
			ReVolt.Track.AINodePriority.RacingLine, //bino_05
			ReVolt.Track.AINodePriority.RacingLine, //bino_10
			ReVolt.Track.AINodePriority.RacingLine, //bino_20
			ReVolt.Track.AINodePriority.RacingLine, //bino_05_2
			ReVolt.Track.AINodePriority.RacingLine, //bino_10_2
			ReVolt.Track.AINodePriority.RacingLine, //bino_20_2
			ReVolt.Track.AINodePriority.RacingLine, //bino_05_LH
			ReVolt.Track.AINodePriority.RacingLine, //bino_10_LH
			ReVolt.Track.AINodePriority.RacingLine, //bino_20_LH
			ReVolt.Track.AINodePriority.RacingLine, //bino_05_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bino_10_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bino_20_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bino_05_2_D
			ReVolt.Track.AINodePriority.RacingLine, //bino_10_2_D
			ReVolt.Track.AINodePriority.RacingLine, //bino_20_2_D
			ReVolt.Track.AINodePriority.RacingLine, //bino_05_2_LH_D
			ReVolt.Track.AINodePriority.RacingLine, //bino_10_2_LH_D
			ReVolt.Track.AINodePriority.RacingLine, //bino_20_2_LH_D
			ReVolt.Track.AINodePriority.RacingLine, //bino_05_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bino_10_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bino_20_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bino_05_2_LH_N
			ReVolt.Track.AINodePriority.RacingLine, //bino_10_2_LH_N
			ReVolt.Track.AINodePriority.RacingLine, //bino_20_2_LH_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_10
			ReVolt.Track.AINodePriority.RacingLine, //bridge_15
			ReVolt.Track.AINodePriority.RacingLine, //bridge_20
			ReVolt.Track.AINodePriority.RacingLine, //bridge_25
			ReVolt.Track.AINodePriority.RacingLine, //bridge_30
			ReVolt.Track.AINodePriority.RacingLine, //bridge_35
			ReVolt.Track.AINodePriority.RacingLine, //bridge_40
			ReVolt.Track.AINodePriority.RacingLine, //bridge_45
			ReVolt.Track.AINodePriority.RacingLine, //bridge_50
			ReVolt.Track.AINodePriority.RacingLine, //bridge_55
			ReVolt.Track.AINodePriority.RacingLine, //bridge_60
			ReVolt.Track.AINodePriority.RacingLine, //bridge_65
			ReVolt.Track.AINodePriority.RacingLine, //bridge_70
			ReVolt.Track.AINodePriority.RacingLine, //bridge_75
			ReVolt.Track.AINodePriority.RacingLine, //bridge_80
			ReVolt.Track.AINodePriority.RacingLine, //bridge_10_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_15_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_20_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_25_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_30_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_35_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_40_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_45_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_50_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_55_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_60_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_65_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_70_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_75_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_80_2
			ReVolt.Track.AINodePriority.RacingLine, //bridge_10_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_15_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_20_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_25_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_30_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_35_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_40_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_45_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_50_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_55_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_60_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_65_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_70_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_75_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_80_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //bridge_10_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_15_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_20_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_25_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_30_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_35_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_40_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_45_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_50_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_55_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_60_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_65_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_70_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_75_2_N
			ReVolt.Track.AINodePriority.RacingLine, //bridge_80_2_N
			ReVolt.Track.AINodePriority.Slowdown15, //c_basic
			ReVolt.Track.AINodePriority.Slowdown15, //c_basic_2
			ReVolt.Track.AINodePriority.Slowdown15, //c_l_r
			ReVolt.Track.AINodePriority.Slowdown15, //c_l_r_2
			ReVolt.Track.AINodePriority.Slowdown15, //c_l_r_LH
			ReVolt.Track.AINodePriority.Slowdown15, //c_l_r_2_LH
			ReVolt.Track.AINodePriority.Slowdown15, //c_l_r_N
			ReVolt.Track.AINodePriority.Slowdown15, //c_l_r_2_N
			ReVolt.Track.AINodePriority.Slowdown20, //c_l_x
			ReVolt.Track.AINodePriority.Slowdown20, //c_l_x_2
			ReVolt.Track.AINodePriority.Slowdown20, //c_l_x_LH
			ReVolt.Track.AINodePriority.Slowdown20, //c_l_x_2_LH
			ReVolt.Track.AINodePriority.Slowdown20, //c_l_x_N
			ReVolt.Track.AINodePriority.Slowdown20, //c_l_x_2_N
			ReVolt.Track.AINodePriority.RacingLine, //c_l_xt
			ReVolt.Track.AINodePriority.RacingLine, //c_l_xt_2
			ReVolt.Track.AINodePriority.RacingLine, //c_l_xt_LH
			ReVolt.Track.AINodePriority.RacingLine, //c_l_xt_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //c_l_xt_N
			ReVolt.Track.AINodePriority.RacingLine, //c_l_xt_2_N
			ReVolt.Track.AINodePriority.RacingLine, //d_r
			ReVolt.Track.AINodePriority.RacingLine, //d_x
			ReVolt.Track.AINodePriority.RacingLine, //d_xt
			ReVolt.Track.AINodePriority.RacingLine, //d_r_2
			ReVolt.Track.AINodePriority.RacingLine, //d_x_2
			ReVolt.Track.AINodePriority.RacingLine, //d_xt_2
			ReVolt.Track.AINodePriority.RacingLine, //h_r
			ReVolt.Track.AINodePriority.RacingLine, //h_x
			ReVolt.Track.AINodePriority.RacingLine, //h_xt
			ReVolt.Track.AINodePriority.RacingLine, //h_r_2
			ReVolt.Track.AINodePriority.RacingLine, //h_x_2
			ReVolt.Track.AINodePriority.RacingLine, //h_xt_2
			ReVolt.Track.AINodePriority.RacingLine, //narrow
			ReVolt.Track.AINodePriority.RacingLine, //narrowf
			ReVolt.Track.AINodePriority.RacingLine, //narrowf_2
			ReVolt.Track.AINodePriority.RacingLine, //p_(0)
			ReVolt.Track.AINodePriority.RacingLine, //p_(1)
			ReVolt.Track.AINodePriority.RacingLine, //p_(2)
			ReVolt.Track.AINodePriority.RacingLine, //p_20_(0)
			ReVolt.Track.AINodePriority.RacingLine, //p_20_(1,bot)
			ReVolt.Track.AINodePriority.RacingLine, //p_20_(1,top)
			ReVolt.Track.AINodePriority.RacingLine, //p_20_(2)
			ReVolt.Track.AINodePriority.Slowdown15, //pc_(0)
			ReVolt.Track.AINodePriority.Slowdown15, //pc_(1)
			ReVolt.Track.AINodePriority.Slowdown15, //pc_(1a)
			ReVolt.Track.AINodePriority.Slowdown15, //pc_(2)
			ReVolt.Track.AINodePriority.RacingLine, //pf
			ReVolt.Track.AINodePriority.RacingLine, //p_(1a)
			ReVolt.Track.AINodePriority.RacingLine, //pf_out
			ReVolt.Track.AINodePriority.RacingLine, //pf_n
			ReVolt.Track.AINodePriority.RacingLine, //r_00
			ReVolt.Track.AINodePriority.RacingLine, //r_05
			ReVolt.Track.AINodePriority.RacingLine, //r_10
			ReVolt.Track.AINodePriority.RacingLine, //r_15
			ReVolt.Track.AINodePriority.RacingLine, //r_20
			ReVolt.Track.AINodePriority.RacingLine, //r_25
			ReVolt.Track.AINodePriority.RacingLine, //r_30
			ReVolt.Track.AINodePriority.RacingLine, //r_00_2
			ReVolt.Track.AINodePriority.RacingLine, //r_05_2
			ReVolt.Track.AINodePriority.RacingLine, //r_10_2
			ReVolt.Track.AINodePriority.RacingLine, //r_15_2
			ReVolt.Track.AINodePriority.RacingLine, //r_20_2
			ReVolt.Track.AINodePriority.RacingLine, //r_25_2
			ReVolt.Track.AINodePriority.RacingLine, //r_30_2
			ReVolt.Track.AINodePriority.RacingLine, //r_05_2_D
			ReVolt.Track.AINodePriority.RacingLine, //r_10_2_D
			ReVolt.Track.AINodePriority.RacingLine, //r_15_2_D
			ReVolt.Track.AINodePriority.RacingLine, //r_20_2_D
			ReVolt.Track.AINodePriority.RacingLine, //r_25_2_D
			ReVolt.Track.AINodePriority.RacingLine, //r_30_2_D
			ReVolt.Track.AINodePriority.RacingLine, //r_05_2_N
			ReVolt.Track.AINodePriority.RacingLine, //r_10_2_N
			ReVolt.Track.AINodePriority.RacingLine, //r_15_2_N
			ReVolt.Track.AINodePriority.RacingLine, //r_20_2_N
			ReVolt.Track.AINodePriority.RacingLine, //r_25_2_N
			ReVolt.Track.AINodePriority.RacingLine, //r_30_2_N
			ReVolt.Track.AINodePriority.RacingLine, //sf_01
			ReVolt.Track.AINodePriority.RacingLine, //sf_02
			ReVolt.Track.AINodePriority.RacingLine, //sf_01_2
			ReVolt.Track.AINodePriority.RacingLine, //sf_02_2
			ReVolt.Track.AINodePriority.RacingLine, //sf_01_LH
			ReVolt.Track.AINodePriority.RacingLine, //sf_02_LH
			ReVolt.Track.AINodePriority.RacingLine, //sf_01_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //sf_02_2_LH
			ReVolt.Track.AINodePriority.RacingLine, //st_02
			ReVolt.Track.AINodePriority.RacingLine, //st_04
			ReVolt.Track.AINodePriority.RacingLine, //st_02_2
			ReVolt.Track.AINodePriority.RacingLine, //st_04_2
			ReVolt.Track.AINodePriority.RacingLine, //X
			ReVolt.Track.AINodePriority.Slowdown15, //zigzag
			ReVolt.Track.AINodePriority.Slowdown15, //zigzag_2
			ReVolt.Track.AINodePriority.RacingLine,	  //jump unit
			ReVolt.Track.AINodePriority.RacingLine, //r_00_2_n
		};


		public static readonly int[] OtherSurface = new[]
		{
			(int)ID.TWM_SPACE, //space_unit
			(int)ID.TWM_START, //start_grid
			(int)ID.TWM_SPACE_B, //space_unit
			(int)ID.TWM_BANK_05_2, //b_05
			(int)ID.TWM_BANK_10_2, //b_10
			(int)ID.TWM_BANK_20_2, //b_20
			(int)ID.TWM_BANK_05, //b_05_2
			(int)ID.TWM_BANK_10, //b_10_2
			(int)ID.TWM_BANK_20, //b_20_2
			(int)ID.TWM_BANK_05_2_LH, //b_05_2_LH
			(int)ID.TWM_BANK_10_2_LH, //b_10_2_LH
			(int)ID.TWM_BANK_20_2_LH, //b_20_2_LH
			(int)ID.TWM_BANK_05_2_N, //b_05_2_N
			(int)ID.TWM_BANK_10_2_N, //b_10_2_N
			(int)ID.TWM_BANK_20_2_N, //b_20_2_N
			(int)ID.TWM_BANK_CORNER_05_2, //bc_05
			(int)ID.TWM_BANK_CORNER_10_2, //bc_10
			(int)ID.TWM_BANK_CORNER_20_2, //bc_20
			(int)ID.TWM_BANK_CORNER_05, //bc_05_2
			(int)ID.TWM_BANK_CORNER_10, //bc_10_2
			(int)ID.TWM_BANK_CORNER_20, //bc_20_2
			(int)ID.TWM_BANK_IN_05_2, //bino_05
			(int)ID.TWM_BANK_IN_10_2, //bino_10
			(int)ID.TWM_BANK_IN_20_2, //bino_20
			(int)ID.TWM_BANK_IN_05, //bino_05_2
			(int)ID.TWM_BANK_IN_10, //bino_10_2
			(int)ID.TWM_BANK_IN_20, //bino_20_2
			(int)ID.TWM_BANK_IN_05_2_LH, //bino_05_LH
			(int)ID.TWM_BANK_IN_10_2_LH, //bino_10_LH
			(int)ID.TWM_BANK_IN_20_2_LH, //bino_20_LH
			(int)ID.TWM_BANK_IN_05_LH, //bino_05_2_LH
			(int)ID.TWM_BANK_IN_10_LH, //bino_10_2_LH
			(int)ID.TWM_BANK_IN_20_LH, //bino_20_2_LH
			(int)ID.TWM_BANK_IN_05_2_D, //bino_05_2_D
			(int)ID.TWM_BANK_IN_10_2_D, //bino_10_2_D
			(int)ID.TWM_BANK_IN_20_2_D, //bino_20_2_D
			(int)ID.TWM_BANK_IN_05_2_LH_D, //bino_05_2_LH_D
			(int)ID.TWM_BANK_IN_10_2_LH_D, //bino_10_2_LH_D
			(int)ID.TWM_BANK_IN_20_2_LH_D, //bino_20_2_LH_D
			(int)ID.TWM_BANK_IN_05_2_N, //bino_05_2_N
			(int)ID.TWM_BANK_IN_10_2_N, //bino_10_2_N
			(int)ID.TWM_BANK_IN_20_2_N, //bino_20_2_N
			(int)ID.TWM_BANK_IN_05_2_LH_N, //bino_05_2_LH_N
			(int)ID.TWM_BANK_IN_10_2_LH_N, //bino_10_2_LH_N
			(int)ID.TWM_BANK_IN_20_2_LH_N, //bino_20_2_LH_N
			(int)ID.TWM_BRIDGE_10_2_N, //bridge_10
			(int)ID.TWM_BRIDGE_15_2_N, //bridge_15
			(int)ID.TWM_BRIDGE_20_2_N, //bridge_20
			(int)ID.TWM_BRIDGE_25_2_N, //bridge_25
			(int)ID.TWM_BRIDGE_30_2_N, //bridge_30
			(int)ID.TWM_BRIDGE_35_2_N, //bridge_35
			(int)ID.TWM_BRIDGE_40_2_N, //bridge_40
			(int)ID.TWM_BRIDGE_45_2_N, //bridge_45
			(int)ID.TWM_BRIDGE_50_2_N, //bridge_50
			(int)ID.TWM_BRIDGE_55_2_N, //bridge_55
			(int)ID.TWM_BRIDGE_60_2_N, //bridge_60
			(int)ID.TWM_BRIDGE_65_2_N, //bridge_65
			(int)ID.TWM_BRIDGE_70_2_N, //bridge_70
			(int)ID.TWM_BRIDGE_75_2_N, //bridge_75
			(int)ID.TWM_BRIDGE_80_2_N, //bridge_80
			(int)ID.TWM_BRIDGE_10_2, //bridge_10_2
			(int)ID.TWM_BRIDGE_15_2, //bridge_15_2
			(int)ID.TWM_BRIDGE_20_2, //bridge_20_2
			(int)ID.TWM_BRIDGE_25_2, //bridge_25_2
			(int)ID.TWM_BRIDGE_30_2, //bridge_30_2
			(int)ID.TWM_BRIDGE_35_2, //bridge_35_2
			(int)ID.TWM_BRIDGE_40_2, //bridge_40_2
			(int)ID.TWM_BRIDGE_45_2, //bridge_45_2
			(int)ID.TWM_BRIDGE_50_2, //bridge_50_2
			(int)ID.TWM_BRIDGE_55_2, //bridge_55_2
			(int)ID.TWM_BRIDGE_60_2, //bridge_60_2
			(int)ID.TWM_BRIDGE_65_2, //bridge_65_2
			(int)ID.TWM_BRIDGE_70_2, //bridge_70_2
			(int)ID.TWM_BRIDGE_75_2, //bridge_75_2
			(int)ID.TWM_BRIDGE_80_2, //bridge_80_2
			(int)ID.TWM_BRIDGE_10_2_LH, //bridge_10_2_LH
			(int)ID.TWM_BRIDGE_15_2_LH, //bridge_15_2_LH
			(int)ID.TWM_BRIDGE_20_2_LH, //bridge_20_2_LH
			(int)ID.TWM_BRIDGE_25_2_LH, //bridge_25_2_LH
			(int)ID.TWM_BRIDGE_30_2_LH, //bridge_30_2_LH
			(int)ID.TWM_BRIDGE_35_2_LH, //bridge_35_2_LH
			(int)ID.TWM_BRIDGE_40_2_LH, //bridge_40_2_LH
			(int)ID.TWM_BRIDGE_45_2_LH, //bridge_45_2_LH
			(int)ID.TWM_BRIDGE_50_2_LH, //bridge_50_2_LH
			(int)ID.TWM_BRIDGE_55_2_LH, //bridge_55_2_LH
			(int)ID.TWM_BRIDGE_60_2_LH, //bridge_60_2_LH
			(int)ID.TWM_BRIDGE_65_2_LH, //bridge_65_2_LH
			(int)ID.TWM_BRIDGE_70_2_LH, //bridge_70_2_LH
			(int)ID.TWM_BRIDGE_75_2_LH, //bridge_75_2_LH
			(int)ID.TWM_BRIDGE_80_2_LH, //bridge_80_2_LH
			(int)ID.TWM_BRIDGE_10, //bridge_10_2_N
			(int)ID.TWM_BRIDGE_15, //bridge_15_2_N
			(int)ID.TWM_BRIDGE_20, //bridge_20_2_N
			(int)ID.TWM_BRIDGE_25, //bridge_25_2_N
			(int)ID.TWM_BRIDGE_30, //bridge_30_2_N
			(int)ID.TWM_BRIDGE_35, //bridge_35_2_N
			(int)ID.TWM_BRIDGE_40, //bridge_40_2_N
			(int)ID.TWM_BRIDGE_45, //bridge_45_2_N
			(int)ID.TWM_BRIDGE_50, //bridge_50_2_N
			(int)ID.TWM_BRIDGE_55, //bridge_55_2_N
			(int)ID.TWM_BRIDGE_60, //bridge_60_2_N
			(int)ID.TWM_BRIDGE_65, //bridge_65_2_N
			(int)ID.TWM_BRIDGE_70, //bridge_70_2_N
			(int)ID.TWM_BRIDGE_75, //bridge_75_2_N
			(int)ID.TWM_BRIDGE_80, //bridge_80_2_N
			(int)ID.TWM_CORNER_SQUARE_2, //c_basic
			(int)ID.TWM_CORNER_SQUARE, //c_basic_2
			(int)ID.TWM_CORNER_REGULAR_2, //c_l_r
			(int)ID.TWM_CORNER_REGULAR, //c_l_r_2
			(int)ID.TWM_CORNER_REGULAR_2_LH, //c_l_r_LH
			(int)ID.TWM_CORNER_REGULAR_LH, //c_l_r_2_LH
			(int)ID.TWM_CORNER_REGULAR_2_N, //c_l_r_N
			(int)ID.TWM_CORNER_REGULAR_N, //c_l_r_2_N
			(int)ID.TWM_CORNER_MEDIUM_2, //c_l_x
			(int)ID.TWM_CORNER_MEDIUM, //c_l_x_2
			(int)ID.TWM_CORNER_MEDIUM_2_LH, //c_l_x_LH
			(int)ID.TWM_CORNER_MEDIUM_LH, //c_l_x_2_LH
			(int)ID.TWM_CORNER_MEDIUM_2_N, //c_l_x_N
			(int)ID.TWM_CORNER_MEDIUM_N, //c_l_x_2_N
			(int)ID.TWM_CORNER_LARGE_2, //c_l_xt
			(int)ID.TWM_CORNER_LARGE, //c_l_xt_2
			(int)ID.TWM_CORNER_LARGE_2_LH, //c_l_xt_LH
			(int)ID.TWM_CORNER_LARGE_LH, //c_l_xt_2_LH
			(int)ID.TWM_CORNER_LARGE_2_N, //c_l_xt_N
			(int)ID.TWM_CORNER_LARGE_N, //c_l_xt_2_N
			(int)ID.TWM_DIP_R_2, //d_r
			(int)ID.TWM_DIP_X_2, //d_x
			(int)ID.TWM_DIP_XT_2, //d_xt
			(int)ID.TWM_DIP_R, //d_r_2
			(int)ID.TWM_DIP_X, //d_x_2
			(int)ID.TWM_DIP_XT, //d_xt_2
			(int)ID.TWM_HUMP_R_2, //h_r
			(int)ID.TWM_HUMP_X_2, //h_x
			(int)ID.TWM_HUMP_XT_2, //h_xt
			(int)ID.TWM_HUMP_R, //h_r_2
			(int)ID.TWM_HUMP_X, //h_x_2
			(int)ID.TWM_HUMP_XT, //h_xt_2
			(int)ID.TWM_NARROW, //narrow
			(int)ID.TWM_NARROWF_2, //narrowf
			(int)ID.TWM_NARROWF, //narrowf_2
			(int)ID.TWM_PIPE_0, //p_(0)
			(int)ID.TWM_PIPE_1, //p_(1)
			(int)ID.TWM_PIPE_2, //p_(2)
			(int)ID.TWM_PIPE_20_0, //p_20_(0)
			(int)ID.TWM_PIPE_20_1_BOT, //p_20_(1,bot)
			(int)ID.TWM_PIPE_20_1_TOP, //p_20_(1,top)
			(int)ID.TWM_PIPE_20_2, //p_20_(2)
			(int)ID.TWM_PIPEC_0, //pc_(0)
			(int)ID.TWM_PIPEC_1, //pc_(1)
			(int)ID.TWM_PIPEC_1A, //pc_(1a)
			(int)ID.TWM_PIPEC_2, //pc_(2)
			(int)ID.TWM_PIPEF, //pf
			(int)ID.TWM_PIPE_1A, //p_(1a)
			(int)ID.TWM_PIPEF_OUT, //pf_out
			(int)ID.TWM_PIPEF_N, //pf_n
			(int)ID.TWM_RAMP_00_2, //r_00
			(int)ID.TWM_RAMP_05_2, //r_05
			(int)ID.TWM_RAMP_10_2, //r_10
			(int)ID.TWM_RAMP_15_2, //r_15
			(int)ID.TWM_RAMP_20_2, //r_20
			(int)ID.TWM_RAMP_25_2, //r_25
			(int)ID.TWM_RAMP_30_2, //r_30
			(int)ID.TWM_RAMP_00, //r_00_2
			(int)ID.TWM_RAMP_05, //r_05_2
			(int)ID.TWM_RAMP_10, //r_10_2
			(int)ID.TWM_RAMP_15, //r_15_2
			(int)ID.TWM_RAMP_20, //r_20_2
			(int)ID.TWM_RAMP_25, //r_25_2
			(int)ID.TWM_RAMP_30, //r_30_2
			(int)ID.TWM_RAMP_05_2_D, //r_05_2_D
			(int)ID.TWM_RAMP_10_2_D, //r_10_2_D
			(int)ID.TWM_RAMP_15_2_D, //r_15_2_D
			(int)ID.TWM_RAMP_20_2_D, //r_20_2_D
			(int)ID.TWM_RAMP_25_2_D, //r_25_2_D
			(int)ID.TWM_RAMP_30_2_D, //r_30_2_D
			(int)ID.TWM_RAMP_05_2_N, //r_05_2_N
			(int)ID.TWM_RAMP_10_2_N, //r_10_2_N
			(int)ID.TWM_RAMP_15_2_N, //r_15_2_N
			(int)ID.TWM_RAMP_20_2_N, //r_20_2_N
			(int)ID.TWM_RAMP_25_2_N, //r_25_2_N
			(int)ID.TWM_RAMP_30_2_N, //r_30_2_N
			(int)ID.TWM_DIAGONAL_01_2, //sf_01
			(int)ID.TWM_DIAGONAL_02_2, //sf_02
			(int)ID.TWM_DIAGONAL_01, //sf_01_2
			(int)ID.TWM_DIAGONAL_02, //sf_02_2
			(int)ID.TWM_DIAGONAL_01_2_LH, //sf_01_LH
			(int)ID.TWM_DIAGONAL_02_2_LH, //sf_02_LH
			(int)ID.TWM_DIAGONAL_01_LH, //sf_01_2_LH
			(int)ID.TWM_DIAGONAL_02_LH, //sf_02_2_LH
			(int)ID.TWM_RUMBLE_02_2, //st_02
			(int)ID.TWM_RUMBLE_04_2, //st_04
			(int)ID.TWM_RUMBLE_02, //st_02_2
			(int)ID.TWM_RUMBLE_04, //st_04_2
			(int)ID.TWM_CROSS_ROAD, //X
			(int)ID.TWM_ZIGZAG_2, //zigzag
			(int)ID.TWM_ZIGZAG, //zigzag_2
			(int)ID.TWM_JUMP,	  //jump unit
			(int)ID.TWM_RAMP_00_2_N, //r_00_2_n
		};

		public static readonly ModuleChanges[] Changes = new[]{
			new ModuleChanges(ID.TWM_SPACE, ID.TWM_SPACE, ID.TWM_SPACE, ID.TWM_SPACE_B, ID.TWM_SPACE_B), //space_unit

			new ModuleChanges(ID.TWM_START, ID.TWM_START, ID.TWM_START, ID.TWM_START, ID.TWM_START), //start_grid

			new ModuleChanges(ID.TWM_SPACE_B, ID.TWM_SPACE_B, ID.TWM_SPACE_B, ID.TWM_SPACE_B, ID.TWM_SPACE_B), //SPACE_B_unit


			new ModuleChanges(ID.TWM_BANK_05, ID.TWM_BANK_IN_20, ID.TWM_BANK_10, ID.TWM_BANK_05, ID.TWM_BANK_05), //b_05
			new ModuleChanges(ID.TWM_BANK_10, ID.TWM_BANK_05, ID.TWM_BANK_20, ID.TWM_BANK_10, ID.TWM_BANK_10), //b_10
			new ModuleChanges(ID.TWM_BANK_20, ID.TWM_BANK_10, ID.TWM_BANK_IN_05_LH, ID.TWM_BANK_20, ID.TWM_BANK_20), //b_20

			new ModuleChanges(ID.TWM_BANK_05_2, ID.TWM_BANK_IN_20_2, ID.TWM_BANK_10_2, ID.TWM_BANK_05_2, ID.TWM_BANK_05_2), //b_05_2
			new ModuleChanges(ID.TWM_BANK_10_2, ID.TWM_BANK_05_2, ID.TWM_BANK_20_2, ID.TWM_BANK_10_2, ID.TWM_BANK_10_2), //b_10_2
			new ModuleChanges(ID.TWM_BANK_20_2, ID.TWM_BANK_10_2, ID.TWM_BANK_IN_05_2, ID.TWM_BANK_20_2, ID.TWM_BANK_20_2), //b_20_2

			new ModuleChanges(ID.TWM_BANK_05_2_LH, ID.TWM_BANK_05_2_LH, ID.TWM_BANK_10_2_LH, ID.TWM_BANK_05_2_LH, ID.TWM_BANK_05_2_LH), //b_05_2_LH
			new ModuleChanges(ID.TWM_BANK_10_2_LH, ID.TWM_BANK_05_2_LH, ID.TWM_BANK_20_2_LH, ID.TWM_BANK_10_2_LH, ID.TWM_BANK_10_2_LH), //b_10_2_LH
			new ModuleChanges(ID.TWM_BANK_20_2_LH, ID.TWM_BANK_10_2_LH, ID.TWM_BANK_20_2_LH, ID.TWM_BANK_20_2_LH, ID.TWM_BANK_20_2_LH), //b_20_2_LH

			new ModuleChanges(ID.TWM_BANK_05_2_N, ID.TWM_BANK_IN_20_2_N, ID.TWM_BANK_10_2_N, ID.TWM_BANK_05_2, ID.TWM_BANK_05_2_LH), //b_05_2_N
			new ModuleChanges(ID.TWM_BANK_10_2_N, ID.TWM_BANK_05_2_N, ID.TWM_BANK_20_2_N, ID.TWM_BANK_10_2, ID.TWM_BANK_10_2_LH), //b_10_2_N
			new ModuleChanges(ID.TWM_BANK_20_2_N, ID.TWM_BANK_10_2_N, ID.TWM_BANK_IN_05_2_LH_N, ID.TWM_BANK_20_2, ID.TWM_BANK_20_2_LH), //b_20_2_N

			new ModuleChanges(ID.TWM_BANK_CORNER_05, ID.TWM_BANK_IN_20_LH, ID.TWM_BANK_CORNER_10, ID.TWM_BANK_CORNER_05, ID.TWM_BANK_CORNER_05), //bc_05
			new ModuleChanges(ID.TWM_BANK_CORNER_10, ID.TWM_BANK_CORNER_05, ID.TWM_BANK_CORNER_20, ID.TWM_BANK_CORNER_10, ID.TWM_BANK_CORNER_10), //bc_10
			new ModuleChanges(ID.TWM_BANK_CORNER_20, ID.TWM_BANK_CORNER_10, ID.TWM_BANK_CORNER_20, ID.TWM_BANK_CORNER_20, ID.TWM_BANK_CORNER_20), //bc_20

			new ModuleChanges(ID.TWM_BANK_CORNER_05_2, ID.TWM_BANK_IN_20_2_LH_N, ID.TWM_BANK_CORNER_10_2, ID.TWM_BANK_CORNER_05_2, ID.TWM_BANK_CORNER_05_2), //bc_05_2
			new ModuleChanges(ID.TWM_BANK_CORNER_10_2, ID.TWM_BANK_CORNER_05_2, ID.TWM_BANK_CORNER_20_2, ID.TWM_BANK_CORNER_10_2, ID.TWM_BANK_CORNER_10_2), //bc_10_2
			new ModuleChanges(ID.TWM_BANK_CORNER_20_2, ID.TWM_BANK_CORNER_10_2, ID.TWM_BANK_CORNER_20_2, ID.TWM_BANK_CORNER_20_2, ID.TWM_BANK_CORNER_20_2), //bc_20_2

			new ModuleChanges(ID.TWM_BANK_IN_05, ID.TWM_BANK_IN_05, ID.TWM_BANK_IN_10, ID.TWM_BANK_IN_05, ID.TWM_BANK_IN_05), //bino_05
			new ModuleChanges(ID.TWM_BANK_IN_10, ID.TWM_BANK_IN_05, ID.TWM_BANK_IN_20, ID.TWM_BANK_IN_10, ID.TWM_BANK_IN_10), //bino_10
			new ModuleChanges(ID.TWM_BANK_IN_20, ID.TWM_BANK_IN_10, ID.TWM_BANK_05, ID.TWM_BANK_IN_20, ID.TWM_BANK_IN_20), //bino_20

			new ModuleChanges(ID.TWM_BANK_IN_05_2, ID.TWM_BANK_IN_05_2, ID.TWM_BANK_IN_10_2, ID.TWM_BANK_IN_05_2, ID.TWM_BANK_IN_05_2), //bino_05
			new ModuleChanges(ID.TWM_BANK_IN_10_2, ID.TWM_BANK_IN_05_2, ID.TWM_BANK_IN_20_2, ID.TWM_BANK_IN_10_2, ID.TWM_BANK_IN_10_2), //bino_10
			new ModuleChanges(ID.TWM_BANK_IN_20_2, ID.TWM_BANK_IN_10_2, ID.TWM_BANK_05_2, ID.TWM_BANK_IN_20_2, ID.TWM_BANK_IN_20_2), //bino_20

			new ModuleChanges(ID.TWM_BANK_IN_05_LH, ID.TWM_BANK_20, ID.TWM_BANK_IN_10_LH, ID.TWM_BANK_IN_05_LH, ID.TWM_BANK_IN_05_LH), //bino_05_LH
			new ModuleChanges(ID.TWM_BANK_IN_10_LH, ID.TWM_BANK_IN_05_LH, ID.TWM_BANK_IN_20_LH, ID.TWM_BANK_IN_10_LH, ID.TWM_BANK_IN_10_LH), //bino_10_LH
			new ModuleChanges(ID.TWM_BANK_IN_20_LH, ID.TWM_BANK_IN_10_LH, ID.TWM_BANK_CORNER_05, ID.TWM_BANK_IN_20_LH, ID.TWM_BANK_IN_20_LH), //bino_20_LH

			new ModuleChanges(ID.TWM_BANK_IN_05_2_LH, ID.TWM_BANK_20, ID.TWM_BANK_IN_10_2_LH, ID.TWM_BANK_IN_05_2_LH, ID.TWM_BANK_IN_05_2_LH), //bino_05_2_LH
			new ModuleChanges(ID.TWM_BANK_IN_10_2_LH, ID.TWM_BANK_IN_05_2_LH, ID.TWM_BANK_IN_20_2_LH, ID.TWM_BANK_IN_10_2_LH, ID.TWM_BANK_IN_10_2_LH), //bino_10_2_LH
			new ModuleChanges(ID.TWM_BANK_IN_20_2_LH, ID.TWM_BANK_IN_10_2_LH, ID.TWM_BANK_CORNER_05_2, ID.TWM_BANK_IN_20_2_LH, ID.TWM_BANK_IN_20_2_LH), //bino_20_2_LH

			new ModuleChanges(ID.TWM_BANK_IN_05_2_D, ID.TWM_BANK_IN_05_2_D, ID.TWM_BANK_IN_05_2_D, ID.TWM_BANK_IN_05_2_D, ID.TWM_BANK_IN_05_2_D), //bino_05_2_D
			new ModuleChanges(ID.TWM_BANK_IN_10_2_D, ID.TWM_BANK_IN_10_2_D, ID.TWM_BANK_IN_10_2_D, ID.TWM_BANK_IN_10_2_D, ID.TWM_BANK_IN_10_2_D), //bino_10_2_D
			new ModuleChanges(ID.TWM_BANK_IN_20_2_D, ID.TWM_BANK_IN_20_2_D, ID.TWM_BANK_IN_20_2_D, ID.TWM_BANK_IN_20_2_D, ID.TWM_BANK_IN_20_2_D), //bino_20_2_D

			new ModuleChanges(ID.TWM_BANK_IN_05_2_LH_D, ID.TWM_BANK_IN_05_2_LH_D, ID.TWM_BANK_IN_05_2_LH_D, ID.TWM_BANK_IN_05_2_LH_D, ID.TWM_BANK_IN_05_2_LH_D), //bino_05_2_LH_D
			new ModuleChanges(ID.TWM_BANK_IN_10_2_LH_D, ID.TWM_BANK_IN_10_2_LH_D, ID.TWM_BANK_IN_10_2_LH_D, ID.TWM_BANK_IN_10_2_LH_D, ID.TWM_BANK_IN_10_2_LH_D), //bino_10_2_LH_D
			new ModuleChanges(ID.TWM_BANK_IN_20_2_LH_D, ID.TWM_BANK_IN_20_2_LH_D, ID.TWM_BANK_IN_20_2_LH_D, ID.TWM_BANK_IN_20_2_LH_D, ID.TWM_BANK_IN_20_2_LH_D), //bino_20_2_LH_D

			new ModuleChanges(ID.TWM_BANK_IN_05_2_N, ID.TWM_BANK_IN_05_2_N, ID.TWM_BANK_IN_10_2_N, ID.TWM_BANK_IN_05_2, ID.TWM_BANK_IN_05_2_D), //bino_05_2_N
			new ModuleChanges(ID.TWM_BANK_IN_10_2_N, ID.TWM_BANK_IN_05_2_N, ID.TWM_BANK_IN_20_2_N, ID.TWM_BANK_IN_10_2, ID.TWM_BANK_IN_10_2_D), //bino_10_2_N
			new ModuleChanges(ID.TWM_BANK_IN_20_2_N, ID.TWM_BANK_IN_10_2_N, ID.TWM_BANK_05_2_N, ID.TWM_BANK_IN_20_2, ID.TWM_BANK_IN_20_2_D), //bino_20_2_N

			new ModuleChanges(ID.TWM_BANK_IN_05_2_LH_N, ID.TWM_BANK_20_2_N, ID.TWM_BANK_IN_10_2_LH_N, ID.TWM_BANK_IN_05_2_LH, ID.TWM_BANK_IN_05_2_LH_D), //bino_05_2_LH_N
			new ModuleChanges(ID.TWM_BANK_IN_10_2_LH_N, ID.TWM_BANK_IN_05_2_LH_N, ID.TWM_BANK_IN_20_2_LH_N, ID.TWM_BANK_IN_10_2_LH, ID.TWM_BANK_IN_10_2_LH_D), //bino_10_2_LH_N
			new ModuleChanges(ID.TWM_BANK_IN_20_2_LH_N, ID.TWM_BANK_IN_10_2_LH_N, ID.TWM_BANK_CORNER_05_2, ID.TWM_BANK_IN_20_2_LH, ID.TWM_BANK_IN_20_2_LH_D), //bino_20_2_LH_N

			new ModuleChanges(ID.TWM_BRIDGE_10, ID.TWM_BRIDGE_10, ID.TWM_BRIDGE_15, ID.TWM_BRIDGE_10, ID.TWM_BRIDGE_10), //bridge_10
			new ModuleChanges(ID.TWM_BRIDGE_15, ID.TWM_BRIDGE_10, ID.TWM_BRIDGE_20, ID.TWM_BRIDGE_15, ID.TWM_BRIDGE_15), //bridge_15
			new ModuleChanges(ID.TWM_BRIDGE_20, ID.TWM_BRIDGE_15, ID.TWM_BRIDGE_25, ID.TWM_BRIDGE_20, ID.TWM_BRIDGE_20), //bridge_20
			new ModuleChanges(ID.TWM_BRIDGE_25, ID.TWM_BRIDGE_20, ID.TWM_BRIDGE_30, ID.TWM_BRIDGE_25, ID.TWM_BRIDGE_25), //bridge_25
			new ModuleChanges(ID.TWM_BRIDGE_30, ID.TWM_BRIDGE_25, ID.TWM_BRIDGE_35, ID.TWM_BRIDGE_30, ID.TWM_BRIDGE_30), //bridge_30
			new ModuleChanges(ID.TWM_BRIDGE_35, ID.TWM_BRIDGE_30, ID.TWM_BRIDGE_40, ID.TWM_BRIDGE_35, ID.TWM_BRIDGE_35), //bridge_35
			new ModuleChanges(ID.TWM_BRIDGE_40, ID.TWM_BRIDGE_35, ID.TWM_BRIDGE_45, ID.TWM_BRIDGE_40, ID.TWM_BRIDGE_40), //bridge_40
			new ModuleChanges(ID.TWM_BRIDGE_45, ID.TWM_BRIDGE_40, ID.TWM_BRIDGE_50, ID.TWM_BRIDGE_45, ID.TWM_BRIDGE_45), //bridge_45
			new ModuleChanges(ID.TWM_BRIDGE_50, ID.TWM_BRIDGE_45, ID.TWM_BRIDGE_55, ID.TWM_BRIDGE_50, ID.TWM_BRIDGE_50), //bridge_50
			new ModuleChanges(ID.TWM_BRIDGE_55, ID.TWM_BRIDGE_50, ID.TWM_BRIDGE_60, ID.TWM_BRIDGE_55, ID.TWM_BRIDGE_55), //bridge_55
			new ModuleChanges(ID.TWM_BRIDGE_60, ID.TWM_BRIDGE_55, ID.TWM_BRIDGE_65, ID.TWM_BRIDGE_60, ID.TWM_BRIDGE_60), //bridge_60
			new ModuleChanges(ID.TWM_BRIDGE_65, ID.TWM_BRIDGE_60, ID.TWM_BRIDGE_70, ID.TWM_BRIDGE_65, ID.TWM_BRIDGE_65), //bridge_65
			new ModuleChanges(ID.TWM_BRIDGE_70, ID.TWM_BRIDGE_65, ID.TWM_BRIDGE_75, ID.TWM_BRIDGE_70, ID.TWM_BRIDGE_70), //bridge_70
			new ModuleChanges(ID.TWM_BRIDGE_75, ID.TWM_BRIDGE_70, ID.TWM_BRIDGE_80, ID.TWM_BRIDGE_75, ID.TWM_BRIDGE_75), //bridge_75
			new ModuleChanges(ID.TWM_BRIDGE_80, ID.TWM_BRIDGE_75, ID.TWM_BRIDGE_80, ID.TWM_BRIDGE_80, ID.TWM_BRIDGE_80), //bridge_80

			new ModuleChanges(ID.TWM_BRIDGE_10_2, ID.TWM_BRIDGE_10_2, ID.TWM_BRIDGE_15_2, ID.TWM_BRIDGE_10_2, ID.TWM_BRIDGE_10_2), //bridge_10_2
			new ModuleChanges(ID.TWM_BRIDGE_15_2, ID.TWM_BRIDGE_10_2, ID.TWM_BRIDGE_20_2, ID.TWM_BRIDGE_15_2, ID.TWM_BRIDGE_15_2), //bridge_15_2
			new ModuleChanges(ID.TWM_BRIDGE_20_2, ID.TWM_BRIDGE_15_2, ID.TWM_BRIDGE_25_2, ID.TWM_BRIDGE_20_2, ID.TWM_BRIDGE_20_2), //bridge_20_2
			new ModuleChanges(ID.TWM_BRIDGE_25_2, ID.TWM_BRIDGE_20_2, ID.TWM_BRIDGE_30_2, ID.TWM_BRIDGE_25_2, ID.TWM_BRIDGE_25_2), //bridge_25_2
			new ModuleChanges(ID.TWM_BRIDGE_30_2, ID.TWM_BRIDGE_25_2, ID.TWM_BRIDGE_35_2, ID.TWM_BRIDGE_30_2, ID.TWM_BRIDGE_30_2), //bridge_30_2
			new ModuleChanges(ID.TWM_BRIDGE_35_2, ID.TWM_BRIDGE_30_2, ID.TWM_BRIDGE_40_2, ID.TWM_BRIDGE_35_2, ID.TWM_BRIDGE_35_2), //bridge_35_2
			new ModuleChanges(ID.TWM_BRIDGE_40_2, ID.TWM_BRIDGE_35_2, ID.TWM_BRIDGE_45_2, ID.TWM_BRIDGE_40_2, ID.TWM_BRIDGE_40_2), //bridge_40_2
			new ModuleChanges(ID.TWM_BRIDGE_45_2, ID.TWM_BRIDGE_40_2, ID.TWM_BRIDGE_50_2, ID.TWM_BRIDGE_45_2, ID.TWM_BRIDGE_45_2), //bridge_45_2
			new ModuleChanges(ID.TWM_BRIDGE_50_2, ID.TWM_BRIDGE_45_2, ID.TWM_BRIDGE_55_2, ID.TWM_BRIDGE_50_2, ID.TWM_BRIDGE_50_2), //bridge_50_2
			new ModuleChanges(ID.TWM_BRIDGE_55_2, ID.TWM_BRIDGE_50_2, ID.TWM_BRIDGE_60_2, ID.TWM_BRIDGE_55_2, ID.TWM_BRIDGE_55_2), //bridge_55_2
			new ModuleChanges(ID.TWM_BRIDGE_60_2, ID.TWM_BRIDGE_55_2, ID.TWM_BRIDGE_65_2, ID.TWM_BRIDGE_60_2, ID.TWM_BRIDGE_60_2), //bridge_60_2
			new ModuleChanges(ID.TWM_BRIDGE_65_2, ID.TWM_BRIDGE_60_2, ID.TWM_BRIDGE_70_2, ID.TWM_BRIDGE_65_2, ID.TWM_BRIDGE_65_2), //bridge_65_2
			new ModuleChanges(ID.TWM_BRIDGE_70_2, ID.TWM_BRIDGE_65_2, ID.TWM_BRIDGE_75_2, ID.TWM_BRIDGE_70_2, ID.TWM_BRIDGE_70_2), //bridge_70_2
			new ModuleChanges(ID.TWM_BRIDGE_75_2, ID.TWM_BRIDGE_70_2, ID.TWM_BRIDGE_80_2, ID.TWM_BRIDGE_75_2, ID.TWM_BRIDGE_75_2), //bridge_75_2
			new ModuleChanges(ID.TWM_BRIDGE_80_2, ID.TWM_BRIDGE_75_2, ID.TWM_BRIDGE_80_2, ID.TWM_BRIDGE_80_2, ID.TWM_BRIDGE_80_2), //bridge_80_2

			new ModuleChanges(ID.TWM_BRIDGE_10_2_LH, ID.TWM_BRIDGE_10_2_LH, ID.TWM_BRIDGE_15_2_LH, ID.TWM_BRIDGE_10_2_LH, ID.TWM_BRIDGE_10_2_LH), //bridge_10_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_15_2_LH, ID.TWM_BRIDGE_10_2_LH, ID.TWM_BRIDGE_20_2_LH, ID.TWM_BRIDGE_15_2_LH, ID.TWM_BRIDGE_15_2_LH), //bridge_15_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_20_2_LH, ID.TWM_BRIDGE_15_2_LH, ID.TWM_BRIDGE_25_2_LH, ID.TWM_BRIDGE_20_2_LH, ID.TWM_BRIDGE_20_2_LH), //bridge_20_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_25_2_LH, ID.TWM_BRIDGE_20_2_LH, ID.TWM_BRIDGE_30_2_LH, ID.TWM_BRIDGE_25_2_LH, ID.TWM_BRIDGE_25_2_LH), //bridge_25_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_30_2_LH, ID.TWM_BRIDGE_25_2_LH, ID.TWM_BRIDGE_35_2_LH, ID.TWM_BRIDGE_30_2_LH, ID.TWM_BRIDGE_30_2_LH), //bridge_30_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_35_2_LH, ID.TWM_BRIDGE_30_2_LH, ID.TWM_BRIDGE_40_2_LH, ID.TWM_BRIDGE_35_2_LH, ID.TWM_BRIDGE_35_2_LH), //bridge_35_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_40_2_LH, ID.TWM_BRIDGE_35_2_LH, ID.TWM_BRIDGE_45_2_LH, ID.TWM_BRIDGE_40_2_LH, ID.TWM_BRIDGE_40_2_LH), //bridge_40_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_45_2_LH, ID.TWM_BRIDGE_40_2_LH, ID.TWM_BRIDGE_50_2_LH, ID.TWM_BRIDGE_45_2_LH, ID.TWM_BRIDGE_45_2_LH), //bridge_45_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_50_2_LH, ID.TWM_BRIDGE_45_2_LH, ID.TWM_BRIDGE_55_2_LH, ID.TWM_BRIDGE_50_2_LH, ID.TWM_BRIDGE_50_2_LH), //bridge_50_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_55_2_LH, ID.TWM_BRIDGE_50_2_LH, ID.TWM_BRIDGE_60_2_LH, ID.TWM_BRIDGE_55_2_LH, ID.TWM_BRIDGE_55_2_LH), //bridge_55_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_60_2_LH, ID.TWM_BRIDGE_55_2_LH, ID.TWM_BRIDGE_65_2_LH, ID.TWM_BRIDGE_60_2_LH, ID.TWM_BRIDGE_60_2_LH), //bridge_60_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_65_2_LH, ID.TWM_BRIDGE_60_2_LH, ID.TWM_BRIDGE_70_2_LH, ID.TWM_BRIDGE_65_2_LH, ID.TWM_BRIDGE_65_2_LH), //bridge_65_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_70_2_LH, ID.TWM_BRIDGE_65_2_LH, ID.TWM_BRIDGE_75_2_LH, ID.TWM_BRIDGE_70_2_LH, ID.TWM_BRIDGE_70_2_LH), //bridge_70_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_75_2_LH, ID.TWM_BRIDGE_70_2_LH, ID.TWM_BRIDGE_80_2_LH, ID.TWM_BRIDGE_75_2_LH, ID.TWM_BRIDGE_75_2_LH), //bridge_75_2_LH
			new ModuleChanges(ID.TWM_BRIDGE_80_2_LH, ID.TWM_BRIDGE_75_2_LH, ID.TWM_BRIDGE_80_2_LH, ID.TWM_BRIDGE_80_2_LH, ID.TWM_BRIDGE_80_2_LH), //bridge_80_2_LH

			new ModuleChanges(ID.TWM_BRIDGE_10_2_N, ID.TWM_BRIDGE_10_2_N, ID.TWM_BRIDGE_15_2_N, ID.TWM_BRIDGE_10_2_N, ID.TWM_BRIDGE_10_2_N), //bridge_10_2_N
			new ModuleChanges(ID.TWM_BRIDGE_15_2_N, ID.TWM_BRIDGE_10_2_N, ID.TWM_BRIDGE_20_2_N, ID.TWM_BRIDGE_15_2_N, ID.TWM_BRIDGE_15_2_N), //bridge_15_2_N
			new ModuleChanges(ID.TWM_BRIDGE_20_2_N, ID.TWM_BRIDGE_15_2_N, ID.TWM_BRIDGE_25_2_N, ID.TWM_BRIDGE_20_2_N, ID.TWM_BRIDGE_20_2_N), //bridge_20_2_N
			new ModuleChanges(ID.TWM_BRIDGE_25_2_N, ID.TWM_BRIDGE_20_2_N, ID.TWM_BRIDGE_30_2_N, ID.TWM_BRIDGE_25_2_N, ID.TWM_BRIDGE_25_2_N), //bridge_25_2_N
			new ModuleChanges(ID.TWM_BRIDGE_30_2_N, ID.TWM_BRIDGE_25_2_N, ID.TWM_BRIDGE_35_2_N, ID.TWM_BRIDGE_30_2_N, ID.TWM_BRIDGE_30_2_N), //bridge_30_2_N
			new ModuleChanges(ID.TWM_BRIDGE_35_2_N, ID.TWM_BRIDGE_30_2_N, ID.TWM_BRIDGE_40_2_N, ID.TWM_BRIDGE_35_2_N, ID.TWM_BRIDGE_35_2_N), //bridge_35_2_N
			new ModuleChanges(ID.TWM_BRIDGE_40_2_N, ID.TWM_BRIDGE_35_2_N, ID.TWM_BRIDGE_45_2_N, ID.TWM_BRIDGE_40_2_N, ID.TWM_BRIDGE_40_2_N), //bridge_40_2_N
			new ModuleChanges(ID.TWM_BRIDGE_45_2_N, ID.TWM_BRIDGE_40_2_N, ID.TWM_BRIDGE_50_2_N, ID.TWM_BRIDGE_45_2_N, ID.TWM_BRIDGE_45_2_N), //bridge_45_2_N
			new ModuleChanges(ID.TWM_BRIDGE_50_2_N, ID.TWM_BRIDGE_45_2_N, ID.TWM_BRIDGE_55_2_N, ID.TWM_BRIDGE_50_2_N, ID.TWM_BRIDGE_50_2_N), //bridge_50_2_N
			new ModuleChanges(ID.TWM_BRIDGE_55_2_N, ID.TWM_BRIDGE_50_2_N, ID.TWM_BRIDGE_60_2_N, ID.TWM_BRIDGE_55_2_N, ID.TWM_BRIDGE_55_2_N), //bridge_55_2_N
			new ModuleChanges(ID.TWM_BRIDGE_60_2_N, ID.TWM_BRIDGE_55_2_N, ID.TWM_BRIDGE_65_2_N, ID.TWM_BRIDGE_60_2_N, ID.TWM_BRIDGE_60_2_N), //bridge_60_2_N
			new ModuleChanges(ID.TWM_BRIDGE_65_2_N, ID.TWM_BRIDGE_60_2_N, ID.TWM_BRIDGE_70_2_N, ID.TWM_BRIDGE_65_2_N, ID.TWM_BRIDGE_65_2_N), //bridge_65_2_N
			new ModuleChanges(ID.TWM_BRIDGE_70_2_N, ID.TWM_BRIDGE_65_2_N, ID.TWM_BRIDGE_75_2_N, ID.TWM_BRIDGE_70_2_N, ID.TWM_BRIDGE_70_2_N), //bridge_70_2_N
			new ModuleChanges(ID.TWM_BRIDGE_75_2_N, ID.TWM_BRIDGE_70_2_N, ID.TWM_BRIDGE_80_2_N, ID.TWM_BRIDGE_75_2_N, ID.TWM_BRIDGE_75_2_N), //bridge_75_2_N
			new ModuleChanges(ID.TWM_BRIDGE_80_2_N, ID.TWM_BRIDGE_75_2_N, ID.TWM_BRIDGE_80_2_N, ID.TWM_BRIDGE_80_2_N, ID.TWM_BRIDGE_80_2_N), //bridge_80_2_N

			new ModuleChanges(ID.TWM_CORNER_SQUARE, ID.TWM_CORNER_SQUARE, ID.TWM_CORNER_SQUARE, ID.TWM_CORNER_SQUARE, ID.TWM_CORNER_SQUARE), //c_basic
			new ModuleChanges(ID.TWM_CORNER_SQUARE_2, ID.TWM_CORNER_SQUARE_2, ID.TWM_CORNER_SQUARE_2, ID.TWM_CORNER_SQUARE_2, ID.TWM_CORNER_SQUARE_2), //c_basic_2

			new ModuleChanges(ID.TWM_CORNER_REGULAR, ID.TWM_CORNER_REGULAR, ID.TWM_CORNER_MEDIUM, ID.TWM_CORNER_REGULAR, ID.TWM_CORNER_REGULAR), //c_l_r
			new ModuleChanges(ID.TWM_CORNER_REGULAR_2, ID.TWM_CORNER_REGULAR_2, ID.TWM_CORNER_MEDIUM_2, ID.TWM_CORNER_REGULAR_2, ID.TWM_CORNER_REGULAR_2), //c_l_r_2
			new ModuleChanges(ID.TWM_CORNER_REGULAR_LH, ID.TWM_CORNER_REGULAR_LH, ID.TWM_CORNER_MEDIUM_LH, ID.TWM_CORNER_REGULAR_LH, ID.TWM_CORNER_REGULAR_LH), //c_l_r_LH
			new ModuleChanges(ID.TWM_CORNER_REGULAR_2_LH, ID.TWM_CORNER_REGULAR_2_LH, ID.TWM_CORNER_MEDIUM_2_LH, ID.TWM_CORNER_REGULAR_2_LH, ID.TWM_CORNER_REGULAR_2_LH), //c_l_r_2_LH
			new ModuleChanges(ID.TWM_CORNER_REGULAR_N, ID.TWM_CORNER_REGULAR_N, ID.TWM_CORNER_MEDIUM_N, ID.TWM_CORNER_REGULAR, ID.TWM_CORNER_REGULAR_LH), //c_l_r_N
			new ModuleChanges(ID.TWM_CORNER_REGULAR_2_N, ID.TWM_CORNER_REGULAR_2_N, ID.TWM_CORNER_MEDIUM_2_N, ID.TWM_CORNER_REGULAR_2_N, ID.TWM_CORNER_REGULAR_2_N), //c_l_r_2_N

			new ModuleChanges(ID.TWM_CORNER_MEDIUM, ID.TWM_CORNER_REGULAR, ID.TWM_CORNER_LARGE, ID.TWM_CORNER_MEDIUM, ID.TWM_CORNER_MEDIUM), //c_l_x
			new ModuleChanges(ID.TWM_CORNER_MEDIUM_2, ID.TWM_CORNER_REGULAR_2, ID.TWM_CORNER_LARGE_2, ID.TWM_CORNER_MEDIUM_2, ID.TWM_CORNER_MEDIUM_2), //c_l_x_2
			new ModuleChanges(ID.TWM_CORNER_MEDIUM_LH, ID.TWM_CORNER_REGULAR_LH, ID.TWM_CORNER_LARGE_LH, ID.TWM_CORNER_MEDIUM_LH, ID.TWM_CORNER_MEDIUM_LH), //c_l_x_LH
			new ModuleChanges(ID.TWM_CORNER_MEDIUM_2_LH, ID.TWM_CORNER_REGULAR_2_LH, ID.TWM_CORNER_LARGE_2_LH, ID.TWM_CORNER_MEDIUM_2_LH, ID.TWM_CORNER_MEDIUM_2_LH), //c_l_x_2_LH
			new ModuleChanges(ID.TWM_CORNER_MEDIUM_N, ID.TWM_CORNER_REGULAR_N, ID.TWM_CORNER_LARGE_N, ID.TWM_CORNER_MEDIUM, ID.TWM_CORNER_MEDIUM_LH), //c_l_x_N
			new ModuleChanges(ID.TWM_CORNER_MEDIUM_2_N, ID.TWM_CORNER_REGULAR_2_N, ID.TWM_CORNER_LARGE_2_N, ID.TWM_CORNER_MEDIUM_2_N, ID.TWM_CORNER_MEDIUM_2_N), //c_l_x_2_N

			new ModuleChanges(ID.TWM_CORNER_LARGE, ID.TWM_CORNER_MEDIUM, ID.TWM_CORNER_LARGE, ID.TWM_CORNER_LARGE, ID.TWM_CORNER_LARGE), //c_l_xt
			new ModuleChanges(ID.TWM_CORNER_LARGE_2, ID.TWM_CORNER_MEDIUM_2, ID.TWM_CORNER_LARGE_2, ID.TWM_CORNER_LARGE_2, ID.TWM_CORNER_LARGE_2), //c_l_xt_2
			new ModuleChanges(ID.TWM_CORNER_LARGE_LH, ID.TWM_CORNER_MEDIUM_LH, ID.TWM_CORNER_LARGE_LH, ID.TWM_CORNER_LARGE_LH, ID.TWM_CORNER_LARGE_LH), //c_l_xt_LH
			new ModuleChanges(ID.TWM_CORNER_LARGE_2_LH, ID.TWM_CORNER_MEDIUM_2_LH, ID.TWM_CORNER_LARGE_2_LH, ID.TWM_CORNER_LARGE_2_LH, ID.TWM_CORNER_LARGE_2_LH), //c_l_xt_2_LH
			new ModuleChanges(ID.TWM_CORNER_LARGE_N, ID.TWM_CORNER_MEDIUM_N, ID.TWM_CORNER_LARGE_N, ID.TWM_CORNER_LARGE, ID.TWM_CORNER_LARGE_LH), //c_l_xt_N
			new ModuleChanges(ID.TWM_CORNER_LARGE_2_N, ID.TWM_CORNER_MEDIUM_2_N, ID.TWM_CORNER_LARGE_2_N, ID.TWM_CORNER_LARGE_2_N, ID.TWM_CORNER_LARGE_2_N), //c_l_xt_2_N

			new ModuleChanges(ID.TWM_DIP_R, ID.TWM_DIP_R, ID.TWM_DIP_X, ID.TWM_DIP_R, ID.TWM_DIP_R), //d_r
			new ModuleChanges(ID.TWM_DIP_X, ID.TWM_DIP_R, ID.TWM_DIP_XT, ID.TWM_DIP_X, ID.TWM_DIP_X), //d_x
			new ModuleChanges(ID.TWM_DIP_XT, ID.TWM_DIP_X, ID.TWM_DIP_XT, ID.TWM_DIP_XT, ID.TWM_DIP_XT), //d_xt
			new ModuleChanges(ID.TWM_DIP_R_2, ID.TWM_DIP_R_2, ID.TWM_DIP_X_2, ID.TWM_DIP_R_2, ID.TWM_DIP_R_2), //d_r_2
			new ModuleChanges(ID.TWM_DIP_X_2, ID.TWM_DIP_R_2, ID.TWM_DIP_XT_2, ID.TWM_DIP_X_2, ID.TWM_DIP_X_2), //d_x_2
			new ModuleChanges(ID.TWM_DIP_XT_2, ID.TWM_DIP_X_2, ID.TWM_DIP_XT_2, ID.TWM_DIP_XT_2, ID.TWM_DIP_XT_2), //d_xt_2

			new ModuleChanges(ID.TWM_HUMP_R, ID.TWM_HUMP_R, ID.TWM_HUMP_X, ID.TWM_HUMP_R, ID.TWM_HUMP_R), //h_r
			new ModuleChanges(ID.TWM_HUMP_X, ID.TWM_HUMP_R, ID.TWM_HUMP_XT, ID.TWM_HUMP_X, ID.TWM_HUMP_X), //h_x
			new ModuleChanges(ID.TWM_HUMP_XT, ID.TWM_HUMP_X, ID.TWM_HUMP_XT, ID.TWM_HUMP_XT, ID.TWM_HUMP_XT), //h_xt
			new ModuleChanges(ID.TWM_HUMP_R_2, ID.TWM_HUMP_R_2, ID.TWM_HUMP_X_2, ID.TWM_HUMP_R_2, ID.TWM_HUMP_R_2), //h_r_2
			new ModuleChanges(ID.TWM_HUMP_X_2, ID.TWM_HUMP_R_2, ID.TWM_HUMP_XT_2, ID.TWM_HUMP_X_2, ID.TWM_HUMP_X_2), //h_x_2
			new ModuleChanges(ID.TWM_HUMP_XT_2, ID.TWM_HUMP_X_2, ID.TWM_HUMP_XT_2, ID.TWM_HUMP_XT_2, ID.TWM_HUMP_XT_2), //h_xt_2

			new ModuleChanges(ID.TWM_NARROW, ID.TWM_NARROW, ID.TWM_NARROWF, ID.TWM_NARROW, ID.TWM_NARROW), //narrow
			new ModuleChanges(ID.TWM_NARROWF, ID.TWM_NARROW, ID.TWM_NARROWF, ID.TWM_NARROWF, ID.TWM_NARROWF), //narrowf
			new ModuleChanges(ID.TWM_NARROWF_2, ID.TWM_NARROWF_2, ID.TWM_NARROWF_2, ID.TWM_NARROWF_2, ID.TWM_NARROWF_2), //narrowf_2

			new ModuleChanges(ID.TWM_PIPE_0, ID.TWM_PIPE_0, ID.TWM_PIPE_0, ID.TWM_PIPE_0, ID.TWM_PIPE_0), //p_0
			new ModuleChanges(ID.TWM_PIPE_1, ID.TWM_PIPE_1, ID.TWM_PIPE_1, ID.TWM_PIPE_1, ID.TWM_PIPE_1), //p_1
			new ModuleChanges(ID.TWM_PIPE_2, ID.TWM_PIPEF_N, ID.TWM_PIPEC_2, ID.TWM_PIPE_2, ID.TWM_PIPE_2), //p_2

			new ModuleChanges(ID.TWM_PIPE_20_0, ID.TWM_PIPE_20_0, ID.TWM_PIPE_20_0, ID.TWM_PIPE_20_0, ID.TWM_PIPE_20_0), //p_20_0
			new ModuleChanges(ID.TWM_PIPE_20_1_BOT, ID.TWM_PIPE_20_1_BOT, ID.TWM_PIPE_20_1_BOT, ID.TWM_PIPE_20_1_BOT, ID.TWM_PIPE_20_1_BOT), //p_20_1_bot
			new ModuleChanges(ID.TWM_PIPE_20_1_TOP, ID.TWM_PIPE_20_1_TOP, ID.TWM_PIPE_20_1_TOP, ID.TWM_PIPE_20_1_TOP, ID.TWM_PIPE_20_1_TOP), //p_20_1_top
			new ModuleChanges(ID.TWM_PIPE_20_2, ID.TWM_PIPEC_2, ID.TWM_PIPE_20_2, ID.TWM_PIPE_20_2, ID.TWM_PIPE_20_2), //p_20_2

			new ModuleChanges(ID.TWM_PIPEC_0, ID.TWM_PIPEC_0, ID.TWM_PIPEC_0, ID.TWM_PIPEC_0, ID.TWM_PIPEC_0), //pc_0

			new ModuleChanges(ID.TWM_PIPEC_1, ID.TWM_PIPEC_1, ID.TWM_PIPEC_1, ID.TWM_PIPEC_1, ID.TWM_PIPEC_1), //pc_1
			new ModuleChanges(ID.TWM_PIPEC_1A, ID.TWM_PIPEC_1A, ID.TWM_PIPEC_1A, ID.TWM_PIPEC_1A, ID.TWM_PIPEC_1A), //pc_1a

			new ModuleChanges(ID.TWM_PIPEC_2, ID.TWM_PIPE_2, ID.TWM_PIPE_20_2, ID.TWM_PIPEC_2, ID.TWM_PIPEC_2), //pc_2

			new ModuleChanges(ID.TWM_PIPEF, ID.TWM_PIPEF, ID.TWM_PIPE_2, ID.TWM_PIPEF, ID.TWM_PIPEF_OUT), //pf

			new ModuleChanges(ID.TWM_PIPE_1A, ID.TWM_PIPE_1A, ID.TWM_PIPE_1A, ID.TWM_PIPE_1A, ID.TWM_PIPE_1A), //p_1a
			new ModuleChanges(ID.TWM_PIPEF_OUT, ID.TWM_PIPEF_OUT, ID.TWM_PIPE_2, ID.TWM_PIPEF_OUT, ID.TWM_PIPEF_OUT), //pf
			new ModuleChanges(ID.TWM_PIPEF_N, ID.TWM_PIPEF_N, ID.TWM_PIPE_2, ID.TWM_PIPEF, ID.TWM_PIPEF_OUT), //pf
			new ModuleChanges(ID.TWM_RAMP_00, ID.TWM_RAMP_00, ID.TWM_RAMP_05, ID.TWM_RAMP_00, ID.TWM_RAMP_00), //r_00
			new ModuleChanges(ID.TWM_RAMP_05, ID.TWM_RAMP_00, ID.TWM_RAMP_10, ID.TWM_RAMP_05, ID.TWM_RAMP_05), //r_05
			new ModuleChanges(ID.TWM_RAMP_10, ID.TWM_RAMP_05, ID.TWM_RAMP_15, ID.TWM_RAMP_10, ID.TWM_RAMP_10), //r_10
			new ModuleChanges(ID.TWM_RAMP_15, ID.TWM_RAMP_10, ID.TWM_RAMP_20, ID.TWM_RAMP_15, ID.TWM_RAMP_15), //r_15
			new ModuleChanges(ID.TWM_RAMP_20, ID.TWM_RAMP_15, ID.TWM_RAMP_25, ID.TWM_RAMP_20, ID.TWM_RAMP_20), //r_20
			new ModuleChanges(ID.TWM_RAMP_25, ID.TWM_RAMP_20, ID.TWM_RAMP_30, ID.TWM_RAMP_25, ID.TWM_RAMP_25), //r_25
			new ModuleChanges(ID.TWM_RAMP_30, ID.TWM_RAMP_25, ID.TWM_RAMP_30, ID.TWM_RAMP_30, ID.TWM_RAMP_30), //r_30

			new ModuleChanges(ID.TWM_RAMP_00_2, ID.TWM_RAMP_00_2, ID.TWM_RAMP_05_2, ID.TWM_RAMP_00_2, ID.TWM_RAMP_00_2), //r_00_2
			new ModuleChanges(ID.TWM_RAMP_05_2, ID.TWM_RAMP_00_2, ID.TWM_RAMP_10_2, ID.TWM_RAMP_05_2, ID.TWM_RAMP_05_2_D), //r_05_2
			new ModuleChanges(ID.TWM_RAMP_10_2, ID.TWM_RAMP_05_2, ID.TWM_RAMP_15_2, ID.TWM_RAMP_10_2, ID.TWM_RAMP_10_2_D), //r_10_2
			new ModuleChanges(ID.TWM_RAMP_15_2, ID.TWM_RAMP_10_2, ID.TWM_RAMP_20_2, ID.TWM_RAMP_15_2, ID.TWM_RAMP_15_2_D), //r_15_2
			new ModuleChanges(ID.TWM_RAMP_20_2, ID.TWM_RAMP_15_2, ID.TWM_RAMP_25_2, ID.TWM_RAMP_20_2, ID.TWM_RAMP_20_2_D), //r_20_2
			new ModuleChanges(ID.TWM_RAMP_25_2, ID.TWM_RAMP_20_2, ID.TWM_RAMP_30_2, ID.TWM_RAMP_25_2, ID.TWM_RAMP_25_2_D), //r_25_2
			new ModuleChanges(ID.TWM_RAMP_30_2, ID.TWM_RAMP_25_2, ID.TWM_RAMP_30_2, ID.TWM_RAMP_30_2, ID.TWM_RAMP_30_2_D), //r_30_2

			new ModuleChanges(ID.TWM_RAMP_05_2_D, ID.TWM_RAMP_05_2_D, ID.TWM_RAMP_05_2_D, ID.TWM_RAMP_05_2_D, ID.TWM_RAMP_05_2_D), //r_05_2_D
			new ModuleChanges(ID.TWM_RAMP_10_2_D, ID.TWM_RAMP_10_2_D, ID.TWM_RAMP_10_2_D, ID.TWM_RAMP_10_2_D, ID.TWM_RAMP_10_2_D), //r_10_2_D
			new ModuleChanges(ID.TWM_RAMP_15_2_D, ID.TWM_RAMP_15_2_D, ID.TWM_RAMP_15_2_D, ID.TWM_RAMP_15_2_D, ID.TWM_RAMP_15_2_D), //r_15_2_D
			new ModuleChanges(ID.TWM_RAMP_20_2_D, ID.TWM_RAMP_20_2_D, ID.TWM_RAMP_20_2_D, ID.TWM_RAMP_20_2_D, ID.TWM_RAMP_20_2_D), //r_20_2_D
			new ModuleChanges(ID.TWM_RAMP_25_2_D, ID.TWM_RAMP_25_2_D, ID.TWM_RAMP_25_2_D, ID.TWM_RAMP_25_2_D, ID.TWM_RAMP_25_2_D), //r_25_2_D
			new ModuleChanges(ID.TWM_RAMP_30_2_D, ID.TWM_RAMP_30_2_D, ID.TWM_RAMP_30_2_D, ID.TWM_RAMP_30_2_D, ID.TWM_RAMP_30_2_D), //r_30_2_D
			new ModuleChanges(ID.TWM_RAMP_05_2_N, ID.TWM_RAMP_00_2_N, ID.TWM_RAMP_10_2_N, ID.TWM_RAMP_05_2, ID.TWM_RAMP_05_2_D), //r_05_2_N
			new ModuleChanges(ID.TWM_RAMP_10_2_N, ID.TWM_RAMP_05_2_N, ID.TWM_RAMP_15_2_N, ID.TWM_RAMP_10_2, ID.TWM_RAMP_10_2_D), //r_10_2_N
			new ModuleChanges(ID.TWM_RAMP_15_2_N, ID.TWM_RAMP_10_2_N, ID.TWM_RAMP_20_2_N, ID.TWM_RAMP_15_2, ID.TWM_RAMP_15_2_D), //r_15_2_N
			new ModuleChanges(ID.TWM_RAMP_20_2_N, ID.TWM_RAMP_15_2_N, ID.TWM_RAMP_25_2_N, ID.TWM_RAMP_20_2, ID.TWM_RAMP_20_2_D), //r_20_2_N
			new ModuleChanges(ID.TWM_RAMP_25_2_N, ID.TWM_RAMP_20_2_N, ID.TWM_RAMP_30_2_N, ID.TWM_RAMP_25_2, ID.TWM_RAMP_25_2_D), //r_25_2_N
			new ModuleChanges(ID.TWM_RAMP_30_2_N, ID.TWM_RAMP_25_2_N, ID.TWM_RAMP_30_2_N, ID.TWM_RAMP_30_2, ID.TWM_RAMP_30_2_D), //r_30_2_N

			new ModuleChanges(ID.TWM_DIAGONAL_01, ID.TWM_DIAGONAL_01_LH, ID.TWM_DIAGONAL_02, ID.TWM_DIAGONAL_01, ID.TWM_DIAGONAL_01), //sf_01
			new ModuleChanges(ID.TWM_DIAGONAL_02, ID.TWM_DIAGONAL_01, ID.TWM_DIAGONAL_02, ID.TWM_DIAGONAL_02, ID.TWM_DIAGONAL_02), //sf_02
			new ModuleChanges(ID.TWM_DIAGONAL_01_2, ID.TWM_DIAGONAL_01_2_LH, ID.TWM_DIAGONAL_02_2, ID.TWM_DIAGONAL_01_2, ID.TWM_DIAGONAL_01_2), //sf_01_2
			new ModuleChanges(ID.TWM_DIAGONAL_02_2, ID.TWM_DIAGONAL_01_2, ID.TWM_DIAGONAL_02_2, ID.TWM_DIAGONAL_02_2, ID.TWM_DIAGONAL_02_2), //sf_02_2
			new ModuleChanges(ID.TWM_DIAGONAL_01_LH, ID.TWM_DIAGONAL_02_LH, ID.TWM_DIAGONAL_01, ID.TWM_DIAGONAL_01_LH, ID.TWM_DIAGONAL_01_LH), //sf_01_LH
			new ModuleChanges(ID.TWM_DIAGONAL_02_LH, ID.TWM_DIAGONAL_02_LH, ID.TWM_DIAGONAL_01_LH, ID.TWM_DIAGONAL_02_LH, ID.TWM_DIAGONAL_02_LH), //sf_02_LH
			new ModuleChanges(ID.TWM_DIAGONAL_01_2_LH, ID.TWM_DIAGONAL_02_2_LH, ID.TWM_DIAGONAL_01_2, ID.TWM_DIAGONAL_01_2_LH, ID.TWM_DIAGONAL_01_2_LH), //sf_01_2_LH
			new ModuleChanges(ID.TWM_DIAGONAL_02_2_LH, ID.TWM_DIAGONAL_02_2_LH, ID.TWM_DIAGONAL_01_2_LH, ID.TWM_DIAGONAL_02_2_LH, ID.TWM_DIAGONAL_02_2_LH), //sf_02_2_LH

			new ModuleChanges(ID.TWM_RUMBLE_02, ID.TWM_RUMBLE_02, ID.TWM_RUMBLE_04, ID.TWM_RUMBLE_02, ID.TWM_RUMBLE_02), //st_02
			new ModuleChanges(ID.TWM_RUMBLE_04, ID.TWM_RUMBLE_02, ID.TWM_RUMBLE_04, ID.TWM_RUMBLE_04, ID.TWM_RUMBLE_04), //st_04
			new ModuleChanges(ID.TWM_RUMBLE_02_2, ID.TWM_RUMBLE_02_2, ID.TWM_RUMBLE_04_2, ID.TWM_RUMBLE_02_2, ID.TWM_RUMBLE_02_2), //st_02_2
			new ModuleChanges(ID.TWM_RUMBLE_04_2, ID.TWM_RUMBLE_02_2, ID.TWM_RUMBLE_04_2, ID.TWM_RUMBLE_04_2, ID.TWM_RUMBLE_04_2), //st_04_2

			new ModuleChanges(ID.TWM_CROSS_ROAD, ID.TWM_CROSS_ROAD, ID.TWM_CROSS_ROAD, ID.TWM_CROSS_ROAD, ID.TWM_CROSS_ROAD), //X

			new ModuleChanges(ID.TWM_ZIGZAG, ID.TWM_ZIGZAG, ID.TWM_ZIGZAG, ID.TWM_ZIGZAG, ID.TWM_ZIGZAG), //zigzag
			new ModuleChanges(ID.TWM_ZIGZAG_2, ID.TWM_ZIGZAG_2, ID.TWM_ZIGZAG_2, ID.TWM_ZIGZAG_2, ID.TWM_ZIGZAG_2), //zigzag_2

			new ModuleChanges(ID.TWM_JUMP, ID.TWM_JUMP, ID.TWM_JUMP, ID.TWM_JUMP, ID.TWM_JUMP),	//jump piece

			new ModuleChanges(ID.TWM_RAMP_00_2_N, ID.TWM_RAMP_00_2_N, ID.TWM_RAMP_05_2_N, ID.TWM_RAMP_00_2, ID.TWM_RAMP_00_2), //r_00_2_N
		};

	}

	public struct ModuleGroup
    {
		public LocString Name;
		public int[] ModuleIDs;

		public bool HasVariant => (ModuleIDs[0] != ModuleIDs[1]);

		public ModuleGroup(LocString name, int moduleId0, int moduleId1)
        {
			Name = name;
			ModuleIDs = new int[] { moduleId0, moduleId1 };
        }

		public ModuleGroup(LocString name, ID moduleId0, ID moduleId1) : this(name, (int)moduleId0, (int)moduleId1)
        {

        }
    }

	public struct ModuleChanges
    {
        public int Original;
        public int PreviousVar;
        public int NextVar;
        public int Forward;
        public int Reverse;

        public ModuleChanges(int original, int previousVar, int nextVar, int forward, int reverse)
        {
            this.Original = original;
            this.PreviousVar = previousVar;
            this.NextVar = nextVar;
            this.Forward = forward;
            this.Reverse = reverse;
        }

		public ModuleChanges(ID original, ID previousVar, ID nextVar, ID forward, ID reverse) :
			this((int)original, (int)previousVar, (int)nextVar, (int)forward, (int)reverse)
		{ }
    }

    public struct ModuleGroupInfo
    {
        public int ModuleID;
        public int AlternateID;
        public int TextIndex;

        public ModuleGroupInfo(int moduleId, int alternateId, int textIndex)
        {
            this.ModuleID = moduleId;
            this.AlternateID = alternateId;
            this.TextIndex = textIndex;
        }
    }
}
