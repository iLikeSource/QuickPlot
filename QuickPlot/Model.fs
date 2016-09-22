namespace QuickPlot

open QuickPlot.DataSources

module Model = 
    type t = 
        { dataSources : DataSources.t 
          series      : ScatterLines.t }
    with 
    member this.pushDataSource(dataSource:Csv.config) =
        { this with dataSources = Array.append this.dataSources [| dataSource |] }
              
    member this.pushSeries(scatterLine:ScatterLine.config) = 
        { this with series = Array.append this.series [| scatterLine |] }
    static member empty() = { dataSources = [||]; series = [||] }
    static member fromFile(path:string) = 
        t.empty().pushDataSource(Csv.config.create(path))
                 .pushSeries(ScatterLine.config.create(0, 1, 2))


    member this.draw() = 
        ScatterLines.draw this.series this.dataSources
