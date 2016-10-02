namespace QuickPlot

open FSharp.Configuration
open Newtonsoft.Json

type tYaml = YamlConfig<"Model.yaml">

module DataSources = 
    
    let addCsvFile (t:tYaml) (csvPath:string) =
        let (xValueColumn, yValueColumn) = (1, 2)
        let dataSource =
            tYaml.DataSources_Item_Type(filePath    = csvPath, 
                                        columns     = new ResizeArray<int>([1; 2]),
                                        skipRows    = 1,
                                        headerAlias = true)
        let series = 
            tYaml.Series_Item_Type(dataSource   = csvPath,
                                   xValueColumn = xValueColumn,
                                   yValueColumn = yValueColumn)
        t.DataSources.Add (dataSource)
        t.Series.Add (series)
        t

    let read (dataSource:tYaml.DataSources_Item_Type) =
        use reader     = new System.IO.StreamReader(dataSource.filePath)
        let skipRows   = dataSource.skipRows
        
        //  Csv読み込み関数
        let rec readCsv 
            (reader:System.IO.StreamReader) 
            (dst:string array list) =
            if reader.EndOfStream then 
                let values = dst |> List.rev |> Array.ofList
                Array.sub values skipRows (values.Length - skipRows)
            else
                let currentData = 
                    reader.ReadLine().Split([| ',' |]) 
                    |> Array.map (fun s -> s.Trim())
                let targetData  = 
                    dataSource.columns 
                    |> Array.ofSeq 
                    |> Array.map (fun column -> 
                        if column <= currentData.Length then 
                            currentData.[column - 1]
                        else
                            ""
                    )
                List.Cons (currentData, dst)
                |> readCsv (reader)
        readCsv reader []


module Model = 

    let empty () : tYaml = 
        tYaml(DataSources = new ResizeArray<tYaml.DataSources_Item_Type>(),
              Series      = new ResizeArray<tYaml.Series_Item_Type>())

    let fromString (s:string) = 
        let x = tYaml()
        x.LoadText(s)
        x

    let toString (t:tYaml) = 
        t.ToString()


