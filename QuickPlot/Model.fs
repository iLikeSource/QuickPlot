namespace QuickPlot

open Newtonsoft.Json
open QuickPlot.DataSources

module Model = 
    type t = 
        { dataSources : DataSources.t 
          series      : ScatterLines.t }
    with 
    member this.pushDataSource(dataSource:Csv.config) =
        { this with dataSources = Array.append this.dataSources [| dataSource |] }
              
    member this.pushSeries(scatterLine:ScatterLine.config) = 
        let line = 
            let lastIndex  = this.dataSources.Length - 1
            let dataSource = this.dataSources.[lastIndex]
            ScatterLine.config.create(dataSourceId=lastIndex, 
                                      xValueColumn=dataSource.readColumns.[0], 
                                      yValueColumn=dataSource.readColumns.[1])
        { this with series = Array.append this.series [| scatterLine |] }

    member this.draw() = 
        ScatterLines.draw this.series this.dataSources
    
    static member empty() = { dataSources = [||]; series = [||] }
    
    static member fromFile(path:string) = 
        t.empty().pushDataSource(Csv.config.create(path))
                 .pushSeries(ScatterLine.config.create(dataSourceId=0, xValueColumn=1, yValueColumn=2))

    static member fromString(s:string) = 
        t.empty().pushDataSource(JsonConvert.DeserializeObject<Csv.config>(s))
                 .pushSeries(ScatterLine.config.create(dataSourceId=0, xValueColumn=1, yValueColumn=2))
        