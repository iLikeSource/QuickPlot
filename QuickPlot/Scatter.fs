namespace QuickPlot

open XPlot.GoogleCharts


module Utilities =  

    type csvReaderConfig = 
        { filePath    : string
          skipRows    : int
          readColumns : int array
          headerAlias : bool }

    type csvContent = 
        { values          : float array array
          headerColumnMap : Map<string, int> }
           
    let fromCsv (columns:int array) (path:string) = 
        use reader = new System.IO.StreamReader(path)
        let rec readCsv (dst:string array list) =
            if reader.EndOfStream then 
                dst |> List.rev |> Array.ofList
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

module Scatter = 

    type graphOptions = 
        { title : string }

    type t = 
        { dataSource : (float * float) array
          options    : graphOptions }
    
    let fromCsv (xColumn, yColumn) (path) = 
        let dataSource =
            Utilities.fromCsv [| xColumn; yColumn |] <| path
            |> Array.choose (fun lineData -> 
                let (xRef, yRef) = (ref 0.0, ref 0.0)
                if System.Double.TryParse(lineData.[0], xRef) &&    
                   System.Double.TryParse(lineData.[1], yRef) then
                    Some (!xRef, !yRef)    
                else
                    None
            )
        { dataSource = dataSource; options= { title = "Title" } }

    let drawFromCsv (path) = 
        let data = fromCsv (1, 2) path
        (Chart.Line(data.dataSource)
         |> Chart.WithWidth  400
         |> Chart.WithHeight 400).Html        

    let sample () = 
        Chart.Line([ (0.0, 0.0); (1.0, 1.5); (2.0, -2.0); (3.0, 4.0) ])
             .Html    


    
    


