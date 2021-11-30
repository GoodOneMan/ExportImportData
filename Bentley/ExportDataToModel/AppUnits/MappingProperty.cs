using ExportDataToModel.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportDataToModel.AppUnits
{
    class MappingProperty
    {
        Dictionary<string, string> propertyMap = new Dictionary<string, string>();

        public MappingProperty()
        {
            string mapping = Resources.PropertyMap;

            string[] mappingArray = mapping.Split('\n');

            foreach(string property in mappingArray)
            {
                string[] map = property.Split('=');

                propertyMap[map[0].Trim()] = map[1].Trim();
            }
        }

        public Dictionary<string, string> GetMapping()
        {
            return propertyMap;
        }

        public static Dictionary<string, string> GetPropertyNameElement()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            //dic["COMPONENT_NAME"] = "Имя компонента";
            //dic["LENGTH_EFFECTIVE"] = "Эффективная длина";
            //dic["DESIGN_LENGTH_CENTER_TO_BRANCH_END_EFFECTIVE"] = "Действительное расчетное расстояние от центра до конца отвода";
            //dic["DESIGN_LENGTH_CENTER_TO_OUTLET_END_EFFECTIVE"] = "Действительное расчетное расстояние от центра до выпускного конца";
            //dic["DESIGN_LENGTH_CENTER_TO_RUN_END_EFFECTIVE"] = "Действительное расчетное расстояние от центра до конца участка трубопровода";
            //dic["WALL_THICKNESS_RUN"] = "Толщина стенки на выходе";
            //dic["UNIT_TYPE"] = "Единицы(шт.,м.,кг.)";
            //dic["ALT_P_MARK"] = "Тип, марка, обозначение";
            //dic["ORDER_NUMBER"] = "Номер заказа";
            //dic["INSULATION_THICKNESS"] = "Толщина изоляции";
            //dic["INSULATION"] = "Материал изоляции";
            //dic["SPECIFICATION"] = "Миникаталог";
            //dic["SCHEDULE"] = "График СМР";
            //dic["NOMINAL_DIAMETER"] = "Номинальный диаметр";
            //dic["SHORT_DESCRIPTION "] = "Короткое описание";
            //dic["SYSTEM"] = "Система";
            //dic["SHOP_FIELD"] = "Поле - Покупное изделие";
            //dic["SERVICE"] = "Продукт";
            //dic["HUB_DEPTH"] = "Длина ступицы";
            //dic["FLAT_LENGTH"] = "Длина фаски";
            //dic["HUB_WIDTH"] = "Ширина ступицы";
            //dic["UNIT_OF_MEASURE"] = "Единица измерения";
            //dic["NAME"] = "Имя";
            //dic["DRY_WEIGHT"] = "Общая масса";
            //dic["UNIT"] = "Единицы";
            //dic["LINENUMBER"] = "Номер линии";
            //dic["LENGTH"] = "Длина";
            //dic["INSIDE_DIAMETER"] = "Внутренний диаметр";
            //dic["NORMAL_OPERATING_PRESSURE"] = "Нормальное рабочее давление";
            //dic["EC_CLASS_NAME"] = "Имя класса EC";
            //dic["DESIGNER"] = "Проектировщик";
            //dic["ELEVATION"] = "Уровень по высоте";
            //dic["TOTAL_WEIGHT"] = "Полный вес";
            //dic["OUTSIDE_DIAMETER_BRANCH_END"] = "ВНЕШНИЙ КОНЕЦ ВЕТВИ ДИАМЕТРА";
            //dic["OUTSIDE_DIAMETER_RUN_END"] = "ВНЕШНИЙ КОНЕЦ БЕГА ДИАМЕТРА";
            //dic["BRANCH_ANGLE"] = "УГОЛ ВЕТВИ";
            //dic["NOMINAL_DIAMETER_BRANCH_END"] = "НОМИНАЛЬНЫЙ КОНЕЦ ВЕТВИ ДИАМЕТРА";
            //dic["NOMINAL_DIAMETER_REDUCING_END"] = "НОМИНАЛЬНЫЙ ДИАМЕТР УМЕНЬШАЯ КОНЕЦ";
            //dic["DESIGN_LENGTH_CENTER_TO_BRANCH_END"] = "ЦЕНТР ДЛИНЫ КОНСТРУКЦИИ К КОНЦУ ВЕТВИ";
            //dic["DESIGN_LENGTH_CENTER_TO_OUTLET_END"] = "ЦЕНТР ДЛИНЫ КОНСТРУКЦИИ К КОНЦУ ВЫХОДА";
            //dic["DESIGN_LENGTH_CENTER_TO_RUN_END"] = "ЦЕНТР ДЛИНЫ КОНСТРУКЦИИ, КОТОР НУЖНО ПОБЕЖАТЬ КОНЕЦ";
            //dic["OUTSIDE_DIAMETER"] = "Наружный диаметр";
            //dic["ALLOWABLE_STARTING_VOLTAGE_PERCENTAGE"] = "ПОЗВОЛЯЕМЫЙ НАЧИНАЯ ПРОЦЕНТ НАПРЯЖЕНИЯ ТОКА";
            //dic["NOMINAL_DIAMETER_RUN_END"] = "Номинальный диаметр на выходе";
            //dic["WALL_THICKNESS_BRANCH"] = "ВЕТВЬ ТОЛЩИНЫ СТЕНЫ";
            //dic["WALL_THICKNESS"] = "Толщина стенки";
            //dic["MATERIAL"] = "Материал";
            //dic["DESIGN_LENGTH_CENTER_TO_RUN_END_PIPING_TEE"] = "ДЛИНА ПО ЦЕНТРУ ДИЗАЙН ДЛЯ ЗАПУСКА КОНЦОВ ТРУБ ТРОЙНИКА";
            //dic["LOWER_LIMIT_LENGTH_CENTER_TO_OUTLET_END_PIPING_TEE"] = "ЦЕНТР ДЛИНЫ НИЖНЕГО ПРЕДЕЛА К ТРОЙНИКУ КОНЦА ВЫХОДА ПРОНЗИТЕЛЬНОМУ";
            //dic["LOWER_LIMIT_LENGTH_CENTER_TO_RUN_END_PIPING_TEE"] = "НИЖНИЙ ПРЕДЕЛ ДЛИНЫ ЦЕНТР ДЛЯ ЗАПУСКА КОНЦОВ ТРУБ ТРОЙНИКА";
            //dic["UPPER_LIMIT_TEE_CENTER_TO_RUN_END_LENGTH"] = "ЦЕНТР ТРОЙНИКА ВЕРХНЕГО ПРЕДЕЛА ДЛЯ БЕГА ДЛИНЫ КОНЦА";
            //dic["UPPER_LIMIT_TEE_CENTER_TO_OUTLET_END_LENGTH"] = "ЦЕНТР ТРОЙНИКА ВЕРХНЕГО ПРЕДЕЛА К ДЛИНЕ КОНЦА ВЫХОДА";
            //dic["LENGTH_CENTER_TO_OUTLET_END_PIPING_TEE_LOWER_LIMIT"] = "ЦЕНТР ДЛИНЫ К НИЖНЕМУ ПРЕДЕЛУ ТРОЙНИКА КОНЦА ВЫХОДА ПРОНЗИТЕЛЬНОМУ";
            //dic["LENGTH_CENTER_TO_OUTLET_END_PIPING_TEE_DESIGN"] = "ЦЕНТР ДЛИНЫ К КОНСТРУКЦИИ ТРОЙНИКА КОНЦА ВЫХОДА ПРОНЗИТЕЛЬНОЙ";
            //dic["LENGTH_CENTER_TO_RUN_END_PIPING_TEE_LOWER_LIMIT"] = "ЦЕНТР ДЛИНЫ ДЛЯ ТОГО ЧТОБЫ ПОБЕЖАТЬ НИЖНИЙ ПРЕДЕЛ ТРОЙНИКА КОНЦА ПРОНЗИТЕЛЬНЫЙ";
            //dic["LENGTH_CENTER_TO_OUTLET_END_PIPING_TEE_UPPER_LIMIT"] = "ЦЕНТР ДЛИНЫ К ВЕРХНЕМУ ПРЕДЕЛУ ТРОЙНИКА КОНЦА ВЫХОДА ПРОНЗИТЕЛЬНОМУ";
            //dic["LENGTH_CENTER_TO_RUN_END_PIPING_TEE_DESIGN"] = "ЦЕНТР ДЛИНЫ ДЛЯ ТОГО ЧТОБЫ ПОБЕЖАТЬ КОНСТРУКЦИЯ ТРОЙНИКА КОНЦА ПРОНЗИТЕЛЬНАЯ";
            //dic["LENGTH_CENTER_TO_RUN_END_PIPING_TEE_MEASURED"] = "ИЗМЕРЕННЫЙ ТРОЙНИК ДЛИНЫ РАЗБИВОЧНЫЙ К КОНЦУ ВЫХОДА ПРОНЗИТЕЛЬНЫЙ";
            //dic["LENGTH_CENTER_TO_OUTLET_END_PIPING_TEE_MEASURED"] = "ИЗМЕРЕННЫЙ ТРОЙНИК ДЛИНЫ РАЗБИВОЧНЫЙ К КОНЦУ ВЫХОДА ПРОНЗИТЕЛЬНЫЙ";
            //dic["LENGTH_CENTER_TO_RUN_END_PIPING_TEE_UPPER_LIMIT"] = "ЦЕНТР ДЛИНЫ ДЛЯ ТОГО ЧТОБЫ ПОБЕЖАТЬ ВЕРХНИЙ ПРЕДЕЛ ТРОЙНИКА КОНЦА ПРОНЗИТЕЛЬНЫЙ";
            //dic["DESIGN_LENGTH_CENTER_TO_OUTLET_END_PIPING_TEE"] = "DESIGN LENGTH CENTER TO OUTLET END PIPING TEE";
            //dic["DESIGN_LENGTH_CENTER_TO_BRANCH_END_PIPING_TEE"] = "ЦЕНТР ДЛИНЫ КОНСТРУКЦИИ К ТРОЙНИКУ КОНЦА ВЕТВИ ПРОНЗИТЕЛЬНОМУ";
            //dic["Description"] = "Описание";

            // OPM
            dic["COMPONENT_NAME"] = "Имя компонента";
            dic["LENGTH_EFFECTIVE"] = "Эффективная длина";
            dic["UNIT_TYPE"] = "Единицы(шт.,м.,кг.)";
            dic["ALT_P_MARK"] = "Тип, марка, обозначение";
            dic["ORDER_NUMBER"] = "Номер заказа";
            dic["INSULATION_THICKNESS"] = "Толщина изоляции";
            dic["INSULATION"] = "Материал изоляции";
            dic["SPECIFICATION"] = "Миникаталог";
            dic["SCHEDULE"] = "График СМР";
            dic["NOMINAL_DIAMETER"] = "Номинальный диаметр";
            dic["SHORT_DESCRIPTION "] = "Короткое описание";
            dic["SYSTEM"] = "Система";
            dic["SHOP_FIELD"] = "Поле - Покупное изделие";
            dic["SERVICE"] = "Продукт";
            dic["UNIT_OF_MEASURE"] = "Единица измерения";
            dic["NAME"] = "Имя";
            dic["DRY_WEIGHT"] = "Общая масса";
            dic["UNIT"] = "Единицы";
            dic["LINENUMBER"] = "Номер линии";
            dic["LENGTH"] = "Длина";
            dic["INSIDE_DIAMETER"] = "Внутренний диаметр";
            dic["NORMAL_OPERATING_PRESSURE"] = "Нормальное рабочее давление";
            dic["EC_CLASS_NAME"] = "Имя класса EC";
            dic["DESIGNER"] = "Проектировщик";
            dic["ELEVATION"] = "Уровень по высоте";
            dic["TOTAL_WEIGHT"] = "Полный вес";
            dic["OUTSIDE_DIAMETER"] = "Наружный диаметр";
            dic["NOMINAL_DIAMETER_RUN_END"] = "Номинальный диаметр на выходе";
            dic["WALL_THICKNESS"] = "Толщина стенки";
            dic["MATERIAL"] = "Материал";
            dic["Description"] = "Описание";
            dic["MAT_STYLE"] = "Марка стали";
            dic["CommodityCode"] = "Код товара";

            // BRCM
            dic["WIDTH"] = "ширина";
            dic["HEIGHT"] = "высота";
            dic["VOLTAGELEVEL"] = "уровень напряжения";
            dic["MANUFACTURER"] = "производитель";
            dic["LENGTH"] = "длина";
            dic["ANGLE"] = "угол";
            dic["RADIUS"] = "радиус";
            dic["ID"] = "ID";

            // OPSE
            dic["Standard"] = "Стандарт";
            dic["CatalogID"] = "Идентификатор каталога";
            dic["CatalogTag"] = "Тег каталога";
            dic["Material"] = "Материал";
            dic["Unit"] = "Единица измерения";
            dic["Service"] = "Продукт";
            dic["Pipeline"] = "Трубопровод";
            dic["Weight"] = "Масса";
            dic["UsedComponents"] = "Используемые Компоненты";
            dic["LinkedSupports"] = "Связанные опоры";
            dic["PipeLines"] = "Трубопроводы";
            dic["PhysicalParameters"] = "Физические Параметры";
            dic["ItemTag"] = "Тег элемент";

            return dic;
        }

        public static Dictionary<string, string> GetPropertyNameLineNumber()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            
            dic["FLUSHING"] = "Промывка";
            dic["BLOW"] = "Продувка";
            dic["STEAMING"] = "Пропарка";
            dic["PREASURE_DROP_PER_HOUR"] = "Процент падения давления в час";
            dic["PREASURE_STRENGTH_TEST_HYDRAULIC"] = "Гидравлическое давление испытания на прочность";
            dic["PREASURE_STRENGTH_TEST_PNEUMATIC"] = "Пневматическое давление испытания на прочность";
            dic["TEST_PRESSURE"] = "Давление испытания на герметичность";
            dic["START_BOUNDARIES"] = "Начало границы участка";
            dic["END_BOUNDARIES"] = "Конец границы участка";
            dic["CATEGORY_GROUP_PIPE"] = "Категория и группа трубопровода";
            dic["CATEGORY_BLOCK"] = "Категория блока";
            dic["CONTROL_WELDS"] = "Контроль сварных швов";
            dic["ELECTRICAL_HEATING"] = "Электрообогрев";
            dic["DENSITY"] = "Плотность";
            dic["NUMBER"] = "Номер";
            dic["SPECIFICATION"] = "Миникаталог";
            dic["UNIT_NAME"] = "Единицы";
            dic["SERVICE_NAME"] = "Продукт";
            dic["SERVICE"] = "Продукт";
            dic["NOMINAL_DIAMETER"] = "Номинальный диаметр";
            dic["INSULATION"] = "Материал изоляции";
            dic["NAME"] = "Имя";
            dic["TRACING"] = "Теплоспутник";
            dic["DESIGN_PRESSURE"] = "Расчетное давление";
            dic["FLOW_RATE"] = "Расход";
            dic["INTERNAL_PRESSURE"] = "Внутреннее давление";
            dic["NOMINAL_DESIGN_TEMPERATURE"] = "Номинальная расчетная температура";
            dic["NORMAL_OPERATING_TEMPERATURE"] = "Нормальная рабочая температура";
            dic["INSULATION_THICKNESS"] = "Толщина изоляции";
            
            return dic;
        }
    }
}
