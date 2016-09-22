namespace QuickPlot

open XPlot.GoogleCharts


module Utilities =  

    type csvContent = 
        { values          : float array array
          headerColumnMap : Map<string, int> }

    type csvReaderConfig = 
        { filePath    : string
          skipRows    : int
          readColumns : int array
          headerAlias : bool }
    with static member create (path) = 
            { filePath    = path
              skipRows    = 0
              readColumns = [| 1; 2 |] 
              headerAlias = false }
           
    let fromCsv (readerConfig:csvReaderConfig) =
        let path     = readerConfig.filePath
        let columns  = readerConfig.readColumns 
        let skipRows = readerConfig.skipRows 
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

module Scatter = 

    type scatterOptions = 
        { lineWidth  : int 
          pointShape : string
          pointSize  : int }
    with static member create () = 
            { lineWidth  = 1
              pointShape = "circle" 
              pointSize  = 3 }
         member __.ChartOptions () =
            let options = new Options(lineWidth  = __.lineWidth, 
                                      pointShape = __.pointShape,
                                      pointSize  = __.pointSize)
            options

    type t = 
        { dataSource : (float * float) array
          options    : scatterOptions }
    
    let withOptions () = 
        let options = new Options()

        options

    let fromCsv (readerConfig) = 
        let dataSource =
            Utilities.fromCsv (readerConfig)
            |> Array.choose (fun lineData -> 
                let (xRef, yRef) = (ref 0.0, ref 0.0)
                if System.Double.TryParse(lineData.[0], xRef) &&    
                   System.Double.TryParse(lineData.[1], yRef) then
                    Some (!xRef, !yRef)    
                else
                    None
            )
        { dataSource = dataSource; options = scatterOptions.create() }

    let drawFromCsv  (readerConfig) = 
        let data = fromCsv (readerConfig) 
        (Chart.Line(data.dataSource)
         |> Chart.WithWidth  400
         |> Chart.WithHeight 400
         |> Chart.WithOptions (data.options.ChartOptions())).Html        

    let sample () = 
        Chart.Line([ (0.0, 0.0); (1.0, 1.5); (2.0, -2.0); (3.0, 4.0) ])
             .Html    


    
    


