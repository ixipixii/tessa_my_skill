<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="81f6010b-9641-4aa5-8897-b8e8603fbf4b" Partition="29f90c69-c1ef-4cbf-b9d5-7fc91cd68c67">
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="1f7ca0d4-1b64-43f8-b973-d14791af51a0" Name="Idx" Type="String(Max) Null">
		<Description>Индекс</Description>
	</SchemePhysicalColumn>
	<Predicate Dbms="SqlServer">[TypeID] != 6</Predicate>
	<Predicate Dbms="PostgreSql">"TypeID" != 6</Predicate>
</SchemeTable>