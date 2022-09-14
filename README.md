# SQLite-Benchmarks
Benchmarking various SQLite use cases.

# FTS5 Lookup Performance

I have two tables; one containing arbitrary content, and another containing an FTS5 (full text) index for the keys in the content table.

According to the [official documentation](https://www.sqlite.org/fts5.html), the prescribed method for linking these two tables in queries appears to be to use the 'External content' method:

```sql
CREATE TABLE data (id TEXT PRIMARY KEY, name TEXT NOT NULL);
CREATE VIRTUAL TABLE idx USING fts5(id, content=data, content_rowid=id);
```

And then querying with an `IN` subquery:

```sql
SELECT name FROM data WHERE ROWID IN (SELECT ROWID FROM idx WHERE idx MATCH '({query})');
```

Benchmark results for this method, using an in-memory database:

```
$ dotnet run
Inserted 500000 records in 41459ms, 12060.107576159578ps
Searched 1000 times in 73ms, 13698.630136986303ps
Searched 1000 times in 77ms, 12987.012987012988ps
Searched 1000 times in 78ms, 12820.51282051282ps
Searched 1000 times in 73ms, 13698.630136986303ps
Searched 1000 times in 73ms, 13698.630136986303ps
Searched 1000 times in 74ms, 13513.513513513513ps
Searched 1000 times in 73ms, 13698.630136986303ps
Searched 1000 times in 74ms, 13513.513513513513ps
Searched 1000 times in 73ms, 13698.630136986303ps
Searched 1000 times in 71ms, 14084.507042253523ps
Searched 1000 times in 76ms, 13157.894736842105ps
Searched 1000 times in 73ms, 13698.630136986303ps
Searched 1000 times in 73ms, 13698.630136986303ps
Searched 1000 times in 75ms, 13333.333333333334ps
Searched 1000 times in 75ms, 13333.333333333334ps
Searched 1000 times in 75ms, 13333.333333333334ps
Searched 1000 times in 75ms, 13333.333333333334ps
Searched 1000 times in 74ms, 13513.513513513513ps
Searched 1000 times in 74ms, 13513.513513513513ps
Searched 1000 times in 74ms, 13513.513513513513ps
```

On disk, with fewer records:

```
$ dotnet run
Inserted 5000 records in 35831ms, 139.5439703050431ps
Searched 1000 times in 209ms, 4784.6889952153115ps
Searched 1000 times in 210ms, 4761.904761904762ps
Searched 1000 times in 206ms, 4854.368932038835ps
Searched 1000 times in 206ms, 4854.368932038835ps
Searched 1000 times in 205ms, 4878.048780487805ps
Searched 1000 times in 204ms, 4901.9607843137255ps
Searched 1000 times in 207ms, 4830.917874396136ps
Searched 1000 times in 211ms, 4739.336492890995ps
Searched 1000 times in 220ms, 4545.454545454545ps
Searched 1000 times in 206ms, 4854.368932038835ps
Searched 1000 times in 208ms, 4807.692307692308ps
Searched 1000 times in 202ms, 4950.49504950495ps
Searched 1000 times in 238ms, 4201.680672268908ps
Searched 1000 times in 263ms, 3802.2813688212927ps
Searched 1000 times in 228ms, 4385.964912280701ps
Searched 1000 times in 202ms, 4950.49504950495ps
Searched 1000 times in 204ms, 4901.9607843137255ps
Searched 1000 times in 217ms, 4608.294930875576ps
Searched 1000 times in 298ms, 3355.7046979865772ps
Searched 1000 times in 248ms, 4032.2580645161293ps
```

Before I read about the 'External content' method, I came up with my own (naive) approach managing the tables independently and using a join:

```sql
CREATE VIRTUAL TABLE idx USING fts5(id);
CREATE TABLE data (id TEXT PRIMARY KEY, name TEXT NOT NULL);
```

```sql
SELECT data.name FROM idx
INNER JOIN data ON idx.id = data.id
WHERE idx MATCH '({query})' ORDER BY name ASC;
```

Benchmark results for this method, using an in-memory database:

```
$ dotnet run
Inserted 500000 records in 42879ms, 11660.719699619862ps
Searched 1000 times in 75ms, 13333.333333333334ps
Searched 1000 times in 72ms, 13888.88888888889ps
Searched 1000 times in 74ms, 13513.513513513513ps
Searched 1000 times in 67ms, 14925.373134328358ps
Searched 1000 times in 69ms, 14492.753623188404ps
Searched 1000 times in 67ms, 14925.373134328358ps
Searched 1000 times in 68ms, 14705.882352941175ps
Searched 1000 times in 70ms, 14285.714285714284ps
Searched 1000 times in 70ms, 14285.714285714284ps
Searched 1000 times in 72ms, 13888.88888888889ps
Searched 1000 times in 73ms, 13698.630136986303ps
Searched 1000 times in 66ms, 15151.51515151515ps
Searched 1000 times in 69ms, 14492.753623188404ps
Searched 1000 times in 69ms, 14492.753623188404ps
Searched 1000 times in 70ms, 14285.714285714284ps
Searched 1000 times in 67ms, 14925.373134328358ps
Searched 1000 times in 70ms, 14285.714285714284ps
Searched 1000 times in 69ms, 14492.753623188404ps
Searched 1000 times in 70ms, 14285.714285714284ps
Searched 1000 times in 68ms, 14705.882352941175ps
```

On disk:

```
$ dotnet run
Inserted 5000 records in 34690ms, 144.13375612568464ps
Searched 1000 times in 207ms, 4830.917874396136ps
Searched 1000 times in 204ms, 4901.9607843137255ps
Searched 1000 times in 204ms, 4901.9607843137255ps
Searched 1000 times in 199ms, 5025.125628140703ps
Searched 1000 times in 200ms, 5000ps
Searched 1000 times in 200ms, 5000ps
Searched 1000 times in 198ms, 5050.50505050505ps
Searched 1000 times in 205ms, 4878.048780487805ps
Searched 1000 times in 202ms, 4950.49504950495ps
Searched 1000 times in 199ms, 5025.125628140703ps
Searched 1000 times in 200ms, 5000ps
Searched 1000 times in 200ms, 5000ps
Searched 1000 times in 200ms, 5000ps
Searched 1000 times in 199ms, 5025.125628140703ps
Searched 1000 times in 202ms, 4950.49504950495ps
Searched 1000 times in 204ms, 4901.9607843137255ps
Searched 1000 times in 201ms, 4975.124378109453ps
Searched 1000 times in 201ms, 4975.124378109453ps
Searched 1000 times in 199ms, 5025.125628140703ps
Searched 1000 times in 199ms, 5025.125628140703ps
```

The performance difference is negligible, but the join method is slightly faster both in memory and on disk.  Furthermore, not using the `content` construct when creating the FTS5 table makes the tables more resilient to changes, as changing the column designated as `content_rowid` will result in a corrupted database.

# Sort Performance

I want to prove (I already kind of know) that sorting results by including an `ORDER BY` predicate in the query is faster than fetching results and sorting in memory.

The initial results were very close, so I tweaked the code to run 1000 times and average the results.

With an in-memory database:

memory:

```
orderby: Average: 16101.471559777603, Min: 10638.297872340425, Max: 18518.51851851852
sqlite: Average: 15239.487507936838, Min: 7751.937984496124, Max: 17857.142857142855
```


On disk (and with a lot fewer records):

```
orderby: Average: 4236.882006072535, Min: 2500, Max: 5000
sqlite: Average: 4099.7076149677805, Min: 1811.5942028985505, Max: 4807.692307692308
```

I didn't evaluate memory usage, but I'd be very surprised if the `OrderBy` method didn't use more memory than `ORDER BY`.