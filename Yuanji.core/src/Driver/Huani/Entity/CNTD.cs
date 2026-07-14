using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 2,func:1--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("MAX统计数据")]
    public struct CNTD
    {
        [Description("给MAX的烟支")]
        public int CigstransferredtoMax;         // 给MAX的烟支

        [Description("从MAX出来的烟支")]
        public int CigtransferredfromMA;         // 从MAX出来的烟支

        [Description("总废烟")]
        public int Totalwaste;         // 总废烟

        [Description("Y2")]
        public int Y2total;         // Y2

        [Description("备用")]
        public int SRMsetpointweightSAM;         // 备用

        [Description("metis")]
        public int Y3total;         // metis

        [Description("备用")]
        public int sesplicesampling_metis;         // 备用

        [Description("备用")]
        public int Y3_spare;         // 备用

        [Description("Y4")]
        public int Y4total;         // Y4

        [Description("Y5缺嘴剔除B42M")]
        public int missing_filter_B42M_Y5;         // Y5缺嘴剔除B42M

        [Description("Y5")]
        public int Y5total;         // Y5

        [Description("卷烟纸拼接段废烟/双支")]
        public int sesplicewaste;         // 卷烟纸拼接段废烟/双支

        [Description("备用")]
        public int Y5spare;         // 备用

        [Description("备用")]
        public int StartStopwaste;         // 备用

        [Description("备用")]
        public int Missingfilter1;         // 备用

        [Description("Y5 B1M未检测到烟")]
        public int B1noCigarettedetectet;         // Y5 B1M未检测到烟

        [Description("备用")]
        public int B14_15noCigarettedetecte;         // 备用

        [Description("水松纸拼接段废烟/双支")]
        public int Tipping_splice_waste;         // 水松纸拼接段废烟/双支

        [Description("备用")]
        public int uncurledPaper;         // 备用

        [Description("Y5未上胶")]
        public int unglued_paper_Y5;         // Y5未上胶

        [Description("Y7 B4M缺嘴")]
        public int cig_ejected_B4M;         // Y7 B4M缺嘴

        [Description("Y8 B5M缺嘴")]
        public int cig_ejected_B5M;         // Y8 B5M缺嘴

        [Description("B4_5缺嘴")]
        public int cig_ejected_B4_5M;         // B4_5缺嘴

        [Description("水松纸盘纸消耗")]
        public int Usedbobbins;         // 水松纸盘纸消耗

        [Description("卷烟纸拼接段废烟")]
        public int SE_splice_waste;         // 卷烟纸拼接段废烟

        [Description("metis")]
        public int Metis_waste;         // metis

        [Description("Y5启动废烟")]
        public int start_waste_Y5;         // Y5启动废烟

        [Description("Y5 B1M未检测到烟")]
        public int B1noCigDet_Y5;         // Y5 B1M未检测到烟

        [Description("Y5 B14M未检测到烟")]
        public int B14noCigDet_Y5;         // Y5 B14M未检测到烟

        [Description("Y5 B15M未检测到烟")]
        public int B15noCigDet_Y5;         // Y5 B15M未检测到烟

        [Description("水松纸拼接段废烟")]
        public int splice_waste_Y5;         // 水松纸拼接段废烟

        [Description("备用")]
        public int uncurled_paper_Y5;         // 备用

        [Description("备用")]
        public int Y6total;         // 备用

        [Description("Y7")]
        public int Y7total;         // Y7

        [Description("Y7 MIDAS太轻烟支剔出")]
        public int MIDAS_too_light_Y7;         // Y7 MIDAS太轻烟支剔出

        [Description("Y7 MIDAS太重烟支剔出")]
        public int MIDAS_too_heavy_Y7;         // Y7 MIDAS太重烟支剔出

        [Description("Y7 MIDAS硬点剔出")]
        public int MIDAS_hardspot_Y7;         // Y7 MIDAS硬点剔出

        [Description("Y7 MIDAS软点剔出")]
        public int MIDAS_softspot_Y7;         // Y7 MIDAS软点剔出

        [Description("Y7 MIDAS轻烟端剔出")]
        public int MIDAS_lightend_Y7;         // Y7 MIDAS轻烟端剔出

        [Description("Y7 IRIS软点剔出")]
        public int IRIS_softspot_Y7;         // Y7 IRIS软点剔出

        [Description("Y7 IRIS轻烟端剔出")]
        public int IRIS_lightend_Y7;         // Y7 IRIS轻烟端剔出

        [Description("Y7 LEO松头")]
        public int LEO_low_density_lit_end_Y7;         // Y7 LEO松头

        [Description("Y7 LEO滤嘴端松头")]
        public int LEO_low_density_Filt_end_Y7;         // Y7 LEO滤嘴端松头

        [Description("Y7 LEO烟支形状")]
        public int LEO_error_cig_shape_Y7;         // Y7 LEO烟支形状

        [Description("Y7 LEO复合滤嘴缺陷")]
        public int LEO_error_multifilter_Y7;         // Y7 LEO复合滤嘴缺陷

        [Description("Y7 HID密封度太低")]
        public int HID_low_airtightness_Y7;         // Y7 HID密封度太低

        [Description("Y7 HID反向密封度太低")]
        public int HID_low_airtightness_reverse_Y7;         // Y7 HID反向密封度太低

        [Description("Y7 HID透气度太低")]
        public int HID_low_total_ventilation_Y7;         // Y7 HID透气度太低

        [Description("Y7 HID透气度太高")]
        public int HID_high_total_ventilation_Y7;         // Y7 HID透气度太高

        [Description("Y7 HID吸阻过低")]
        public int HID_low_pressure_drop_Y7;         // Y7 HID吸阻过低

        [Description("Y7 HID吸阻过高")]
        public int HID_high_pressure_drop_Y7;         // Y7 HID吸阻过高

        [Description("Y8")]
        public int Y8total;         // Y8

        [Description("Y8 MIDAS太轻烟支剔出")]
        public int MIDAS_too_light_Y8;         // Y8 MIDAS太轻烟支剔出

        [Description("Y8 MIDAS太重烟支剔出")]
        public int MIDAS_too_heavy_Y8;         // Y8 MIDAS太重烟支剔出

        [Description("Y8 MIDAS硬点剔出")]
        public int MIDAS_hardspot_Y8;         // Y8 MIDAS硬点剔出

        [Description("Y8 MIDAS软点剔出")]
        public int MIDAS_softspot_Y8;         // Y8 MIDAS软点剔出

        [Description("Y8 MIDAS轻烟端剔出")]
        public int MIDAS_lightend_Y8;         // Y8 MIDAS轻烟端剔出

        [Description("Y8 IRIS软点剔出")]
        public int IRIS_softspot_Y8;         // Y8 IRIS软点剔出

        [Description("Y8 IRIS轻烟端剔出")]
        public int IRIS_lightend_Y8;         // Y8 IRIS轻烟端剔出

        [Description("Y8 LEO松头")]
        public int LEO_low_density_lit_end_Y8;         // Y8 LEO松头

        [Description("Y8 LEO滤嘴端松头")]
        public int LEO_low_density_Filt_end_Y8;         // Y8 LEO滤嘴端松头

        [Description("Y8 LEO烟支形状")]
        public int LEO_error_cig_shape_Y8;         // Y8 LEO烟支形状

        [Description("Y8 LEO复合滤嘴缺陷")]
        public int LEO_error_multifilter_Y8;         // Y8 LEO复合滤嘴缺陷

        [Description("Y8 HID密封度太低")]
        public int HID_low_airtightness_Y8;         // Y8 HID密封度太低

        [Description("Y8 HID反向密封度太低")]
        public int HID_low_airtightness_reverse_Y8;         // Y8 HID反向密封度太低

        [Description("Y8 HID透气度太低")]
        public int HID_low_total_ventilation_Y8;         // Y8 HID透气度太低

        [Description("Y8 HID透气度太高")]
        public int HID_high_total_ventilation_Y8;         // Y8 HID透气度太高

        [Description("Y8 HID吸阻过低")]
        public int HID_low_pressure_drop_Y8;         // Y8 HID吸阻过低

        [Description("Y8 HID吸阻过高")]
        public int HID_high_pressure_drop_Y8;         // Y8 HID吸阻过高

        [Description("Y9")]
        public int y9total;         // Y9

        [Description("Y10")]
        public int y10total;         // Y10

        [Description("水松纸拼接头")]
        public int tippingsplices;         // 水松纸拼接头

        [Description("卷烟纸拼接头")]
        public int SE_splice;         // 卷烟纸拼接头

        [Description("备用")]
        public int Droppedcigarettes;         // 备用

        [Description("单烟支嘴棒数量B42M")]
        public int Filter_plugs;         // 单烟支嘴棒数量B42M

        [Description("Y5实时废品率")]
        public float actual_waste_Y5;         // Y5实时废品率

        [Description("Y7实时废品率")]
        public float actual_waste_Y7;         // Y7实时废品率

        [Description("Y8实时废品率")]
        public float actual_waste_Y8;         // Y8实时废品率

        [Description("备用")]
        public int LEO_uninspected_cig;         // 备用

        [Description("备用")]
        public int HID_uninspected_cig;         // 备用

        [Description("Y7紧头位置")]
        public int SE_densed_end_waste_Y7;         // Y7紧头位置

        [Description("Y8紧头位置")]
        public int SE_densed_end_waste_Y8;         // Y8紧头位置

        [Description("Y7 SE纸关断")]
        public int SE_paper_off_Y7;         // Y7 SE纸关断

        [Description("Y8 SE纸关断")]
        public int SE_paper_off_Y8;         // Y8 SE纸关断

        [Description("Y7 MAX未上胶")]
        public int MAX_NoGlue_Y7;         // Y7 MAX未上胶

        [Description("Y8 MAX未上胶")]
        public int MAX_NoGlue_Y8;         // Y8 MAX未上胶

        [Description("备用")]
        public int MAX_NoCharCoalSegment_Y7;         // 备用

        [Description("备用")]
        public int MAX_NoCharCoalSegment_Y8;         // 备用

        [Description("备用")]
        public int MAX_Totalwaste;         // 备用

        [Description("备用")]
        public int MAX_NTRM_Waste_Y17;         // 备用

        [Description("备用")]
        public int MAX_NTRM_Waste_Y18;         // 备用

        [Description("Y7手动剔除")]
        public int MAX_ManualEjection_Y7;         // Y7手动剔除

        [Description("Y8手动剔除")]
        public int MAX_ManualEjection_Y8;         // Y8手动剔除

        [Description("Y7下游机堵塞剔除")]
        public int MAX_DownstreamEjection_Y7;         // Y7下游机堵塞剔除

        [Description("Y8下游机堵塞剔除")]
        public int MAX_DownstreamEjection_Y8;         // Y8下游机堵塞剔除

        [Description("备用")]
        public int missing_filter_B43M_Y5;         // 备用

        [Description("Y7烟支全外观")]
        public int B16_cigallvision_Y7;         // Y7烟支全外观

        [Description("Y8烟支全外观")]
        public int B17_cigallvision_Y8;         // Y8烟支全外观

        [Description("双长嘴棒数量B42M")]
        public int Double_filter_plug;         // 双长嘴棒数量B42M

        [Description("整长嘴棒数量B42M")]
        public int filter_rods;         // 整长嘴棒数量B42M

        [Description("Y5剔空槽Y2")]
        public int C_RejectedCigarettes;         // Y5剔空槽Y2

        [Description("备用")]
        public int jamdownstraem;         // 备用

        [Description("备用")]
        public int C_ExternalWaste;         // 备用

        [Description("备用")]
        public int NTRM_sampling_Y9;         // 备用

        [Description("备用")]
        public int NTRM_sampling_Y10;         // 备用

        [Description("手动剔除废烟")]
        public int C_ManualEjection;         // 手动剔除废烟

        [Description("备用")]
        public int C_RollBlockJam;         // 备用

        [Description("备用")]
        public int C_RollBlockJamStart;         // 备用

        [Description("备用")]
        public int C_RollBlockJamSplice;         // 备用

        [Description("Y7缺陷钢印")]
        public int CSIS_Print_Waste_Y7;         // Y7缺陷钢印

        [Description("Y7钢印残缺")]
        public int CSIS_Print_Incomplete_Y7;         // Y7钢印残缺

        [Description("Y7钢印脏污")]
        public int CSIS_Print_Dirty_Y7;         // Y7钢印脏污

        [Description("Y7钢印野墨")]
        public int CSIS_Print_Redundant_Y7;         // Y7钢印野墨

        [Description("Y7深浅不一")]
        public int CSIS_Print_Different_depth_Y7;         // Y7深浅不一

        [Description("Y7位置偏差")]
        public int CSIS_Print_Position_Y7;         // Y7位置偏差

        [Description("Y7油墨过浓")]
        public int CSIS_Print_Too_thick_Y7;         // Y7油墨过浓

        [Description("Y7油墨过淡")]
        public int CSIS_Print_Too_light_Y7;         // Y7油墨过淡

        [Description("Y7套色位置")]
        public int CSIS_Chromatic_position_Y7;         // Y7套色位置

        [Description("Y7套色钢印水平位置")]
        public int CSIS_Chromatic_horizontal_position_Y7;         // Y7套色钢印水平位置

        [Description("Y7套色钢印垂直位置")]
        public int CSIS_Chromatic_vertical_position_Y7;         // Y7套色钢印垂直位置

        [Description("Y7燃烧端位置")]
        public int CSIS_Combustion_end_position_Y7;         // Y7燃烧端位置

        [Description("Y8缺陷钢印")]
        public int CSIS_Print_Waste_Y8;         // Y8缺陷钢印

        [Description("Y8钢印残缺")]
        public int CSIS_Print_Incomplete_Y8;         // Y8钢印残缺

        [Description("Y8钢印脏污")]
        public int CSIS_Print_Dirty_Y8;         // Y8钢印脏污

        [Description("Y8钢印野墨")]
        public int CSIS_Print_Redundant_Y8;         // Y8钢印野墨

        [Description("Y8深浅不一")]
        public int CSIS_Print_Different_depth_Y8;         // Y8深浅不一

        [Description("Y8位置偏差")]
        public int CSIS_Print_Position_Y8;         // Y8位置偏差

        [Description("Y8油墨过浓")]
        public int CSIS_Print_Too_thick; //Y8油墨过浓

        [Description("Y8油墨过淡")]
        public int CSIS_Print_Too_light_Y8; //Y8油墨过淡


        [Description("Y8套色位置")]
        public int CSIS_Chromatic_position_Y8;         // Y8套色位置

        [Description("Y8套色钢印水平位置")]
        public int CSIS_Chromatic_horizontal_position_Y8;         // Y8套色钢印水平位置

        [Description("Y8套色钢印垂直位置")]
        public int CSIS_Chromatic_vertical_position_Y8; //Y8套色钢印垂直位置

        [Description("Y8燃烧端位置")]
        public int CSIS_Combustion_end_position_Y8; //Y8燃烧端位置
    }
}