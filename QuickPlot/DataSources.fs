namespace QuickPlot.DataSources

open XPlot.GoogleCharts
open FSharp.Configuration

module Csv =  


    type content = 
        { values          : float array array
          headerColumnMap : Map<string, int> }

    //type config = 
    //    { filePath    : string
    //      skipRows    : int
    //      readColumns : int array
    //      headerAlias : bool }
    type config = YamlConfig<"CsvConfig.yaml">

    let create (csvPath) = 
        config(filePath     = csvPath, 
               skipRows     = 0,             
               xValueColumn = 1,
               yValueColumn = 2,
               headerAlias  = false)
           
    let fromFile (config:config) =
        let path         = config.filePath
        let xValueColumn = config.xValueColumn
        let yValueColumn = config.yValueColumn 
        let skipRows     = config.skipRows 
        use reader   = new System.IO.StreamReader(path)
        let rec readCsv (dst:string array list) =
            if reader.EndOfStream then 
                let values = dst |> List.rev |> Array.ofList
                Array.sub values skipRows (values.Length - skipRows)
            else
                let currentData = 
                    reader.ReadLine().Split([| ',' |]) 
                    |> Array.map (fun s -> s.Trim())
                let targetData  = [| xValueColumn; yValueColumn|] |> Array.map (fun column -> 
                    if column <= currentData.Length then 
                        currentData.[column - 1]
                    else
                        ""
                )
                List.Cons (currentData, dst)
                |> readCsv
        readCsv []

module DataSources = 
    

    type t = Csv.config array

    let toJson(t:t) = Newtonsoft.Json.JsonConvert.SerializeObject(t)

    let toYaml(t:t) = 
        t |> Array.map (fun config ->
            config.
        )
     