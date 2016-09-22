namespace QuickPlot

open XPlot.GoogleCharts


module ScatterLine = 

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

    type config = 
        { dataSourceId : int
          xValueColumn : int
          yValueColumn : int
          xValueHeader : string option
          yValueHeader : string option
          options      : scatterOptions }
    with static member create (dataSourceId, xValueColumn, yValueColumn) = 
            { dataSourceId = dataSourceId
              xValueColumn = xValueColumn
              yValueColumn = yValueColumn
              xValueHeader = None 
              yValueHeader = None 
              options      = scatterOptions.create() }
    
    let withOptions () = 
        let options = new Options()

        options

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
        (data, config.options) 

    let drawFromCsv  (config:config) (sources:DataSources.DataSources.t) = 
        let (data, options) = fromSource config sources
        (Chart.Line(data)
         |> Chart.WithWidth  400
         |> Chart.WithHeight 400
         |> Chart.WithOptions (options.ChartOptions())).Html        

    let sample () = 
        Chart.Line([ (0.0, 0.0); (1.0, 1.5); (2.0, -2.0); (3.0, 4.0) ])
             .Html    


module ScatterLines = 
    
    type t = ScatterLine.config array    
    


