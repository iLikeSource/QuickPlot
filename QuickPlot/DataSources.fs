namespace QuickPlot.DataSources

open XPlot.GoogleCharts


module Csv =  

    type content = 
        { values          : float array array
          headerColumnMap : Map<string, int> }

    type config = 
        { filePath    : string
          skipRows    : int
          readColumns : int array
          headerAlias : bool }
    with static member create (path) = 
            { filePath    = path
              skipRows    = 0
              readColumns = [| 1; 2 |] 
              headerAlias = false }
           
    let fromFile (config:config) =
        let path     = config.filePath
        let columns  = config.readColumns 
        let skipRows = config.skipRows 
        use reader   = new System.IO.StreamReader(path)
        let rec readCsv (dst:string array list) =
            if reader.EndOfStream then 
                let values = dst |> List.rev |> Array.ofList
                Array.sub values skipRows (values.Length - skipRows)
            else
                let currentData = 
                    reader.ReadLine().Split([| ',' |]) 
                    |> Array.map (fun s -> s.Trim())
                let targetData  = columns |> Array.map (fun column -> 
                    if column <= currentData.Length then 
                        currentData.[column - 1]
                    else
                        ""
                )
                List.Cons (currentData, dst)
                |> readCsv
        readCsv []

module DataSources = 
    
    type t = 
        { csvSources : (Csv.config * Csv.config) array }