namespace QuickPlot

open System.Collections.Generic
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

    let fromSource (dataSource:tYaml.DataSources_Item_Type) = 
        let data =
            DataSources.read (dataSource)
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
    

    let draw (dataSources:IList<tYaml.DataSources_Item_Type>) = 
        (dataSources
         |> Array.ofSeq
         |> Array.map (fun dataSource -> ScatterLine.fromSource (dataSource))
         |> Chart.Line    
         |> Chart.WithWidth  400
         |> Chart.WithHeight 400).Html        


