namespace QuickPlot

open XPlot.GoogleCharts


module ScatterLine = 


    type config = 
        { dataSourceId : int
          xValueColumn : int
          yValueColumn : int
          xValueHeader : string option
          yValueHeader : string option }
    with static member create (dataSourceId, xValueColumn, yValueColumn) = 
            { dataSourceId = dataSourceId
              xValueColumn = xValueColumn
              yValueColumn = yValueColumn
              xValueHeader = None 
              yValueHeader = None }

    let fromSource (config:config) (sources:DataSources.DataSources.t) = 
        let data =
            let config = sources.[config.dataSourceId]
            DataSources.Csv.fromFile (config)
            |> Array.choose (fun lineData -> 
                let (xRef, yRef) = (ref 0.0, ref 0.0)
                if System.Double.TryParse(lineData.[0], xRef) &&    
                   System.Double.TryParse(lineData.[1], yRef) then
                    Some (!xRef, !yRef)    
                else
                    None
            )
        data 

    let sample () = 
        Chart.Line([ (0.0, 0.0); (1.0, 1.5); (2.0, -2.0); (3.0, 4.0) ])
             .Html    


module ScatterLines = 
    
    type t = ScatterLine.config array    
    

    let draw (lineConfigs:t) (sources:DataSources.DataSources.t) = 
        (lineConfigs
         |> Array.map (fun config -> ScatterLine.fromSource config sources)
         |> Chart.Line    
         |> Chart.WithWidth  400
         |> Chart.WithHeight 400).Html        


