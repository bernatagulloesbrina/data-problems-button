// '2021-05-26 / B.Agullo / 
// by Bernat Agulló
// www.esbrina-ba.com

//select the measures that counts the number of "data problems" the model has and then run the script

//change the next 5 string variables to your choice

string navigationTableName = "Navigation"; 

string buttonTextMeasureName = "Button Text";
string buttonTextPattern = "There are # data problems"; 

string buttonBackgroundMeasureName = "Button Background"; 
string buttonNavigationMeasureName = "Button Navigation"; 
string thereAreDataProblemsMeasureName = "There are data problems"; 
string dataProblemsSheetName = "Data Problems"; 

//colors will be created if not present
string buttonColorMeasureNameWhenVisible = "bad"; 
string buttonColorMeasureValueWhenVisible = "\"#D64554\""; 
string buttonColorMeasureNameWhenInvisible = "transparent"; 

//
// ----- do not modify script below this line -----
//

if (Selected.Measures.Count != 1) {
    Error("Select one and only one measure");
    return;
};

if(Model.Tables.Any(Table => Table.Name == navigationTableName)) {
    Error(navigationTableName + " already exists!");
    return; 
};

//prepare array to iterate on new measure names 
string[] newMeasureNames = 
    {
        buttonTextMeasureName,
        buttonBackgroundMeasureName,
        buttonNavigationMeasureName,
        thereAreDataProblemsMeasureName
    };

//check none of the new measure names already exist as such 
foreach(string measureName in newMeasureNames) {
    if(Model.AllMeasures.Any(Measure => Measure.Name == measureName)) {
        Error(measureName + " already exists!"); 
        return;
    };
};
    
var dataProblemsMeasure = Selected.Measure; 

string navigationTableExpression = 
    "Navigation = FILTER({1},[Value] = 0)";

var navigationTable = 
    Model.AddCalculatedTable(navigationTableName,navigationTableExpression);
    
navigationTable.FormatDax(); 
navigationTable.Description = 
    "Table to store the measures for the dynamic button that leads to the data problems sheet";

navigationTable.IsHidden = true;     

if(!Model.AllMeasures.Any(Measure => Measure.Name == buttonColorMeasureNameWhenVisible)) {
    navigationTable.AddMeasure(buttonColorMeasureNameWhenVisible,buttonColorMeasureValueWhenVisible);
};

if(!Model.AllMeasures.Any(Measure => Measure.Name == buttonColorMeasureNameWhenInvisible)) {
    navigationTable.AddMeasure(buttonColorMeasureNameWhenInvisible,"\"#FFFFFF00\"");
};


string thereAreDataProblemsMeasureExpression = 
    "[" + dataProblemsMeasure.Name + "]>0";


var thereAreDataProblemsMeasure = 
    navigationTable.AddMeasure(
        thereAreDataProblemsMeasureName,
        thereAreDataProblemsMeasureExpression
    );

thereAreDataProblemsMeasure.FormatDax(); 
thereAreDataProblemsMeasure.Description = "Boolean measure, if true, the button leading to data problems sheet should show (internal use only)" ;
 
string buttonBackgroundMeasureExpression = 
    "VAR colorCode = " + 
    "    IF(" + 
    "        [" + thereAreDataProblemsMeasureName + "]," + 
    "        [" + buttonColorMeasureNameWhenVisible + "]," + 
    "        [" + buttonColorMeasureNameWhenInvisible + "]" + 
    "    )" + 
    "RETURN " + 
    "    FORMAT(colorCode,\"@\")";
    
var buttonBackgroundMeasure = 
    navigationTable.AddMeasure(
        buttonBackgroundMeasureName,
        buttonBackgroundMeasureExpression
    );
    
buttonBackgroundMeasure.FormatDax(); 
buttonBackgroundMeasure.Description = "Use this measure for conditional formatting of button background";  

string buttonNavigationMeasureExpression = 
    "IF(" + 
    "    [" + thereAreDataProblemsMeasureName + "]," + 
    "    \"" + dataProblemsSheetName + "\"," + 
    "    \"\"" + 
    ")";


var buttonNavigationMeasure = 
    navigationTable.AddMeasure(
        buttonNavigationMeasureName,
        buttonNavigationMeasureExpression
    );
    
buttonNavigationMeasure.FormatDax(); 
buttonNavigationMeasure.Description = "Use this measure for conditional page navigation";  


string buttonTextMeasureExpression = 
    "IF(" + 
    "    [" + thereAreDataProblemsMeasureName + "]," + 
    "    SUBSTITUTE(\"" + buttonTextPattern + "\",\"#\",FORMAT([" + dataProblemsMeasure.Name + "],0))," + 
    "    \"\"" + 
    ")";
    
    
var buttonTextMeasure = 
    navigationTable.AddMeasure(
        buttonTextMeasureName,
        buttonTextMeasureExpression
    );
    
buttonTextMeasure.FormatDax(); 
buttonTextMeasure.Description = "Use this measure for dynamic button text";  

dataProblemsMeasure.MoveTo(navigationTable);
    