<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="6c977939-bbfc-456f-a133-f1c2244e3cc3" Partition="29f90c69-c1ef-4cbf-b9d5-7fc91cd68c67">
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="6841fc21-5dcb-4805-854a-933b3ab4ebb9" Name="ExtID" Type="Guid Null">
		<Description>Уникальный ID из системы заказчика</Description>
	</SchemePhysicalColumn>
	<Predicate Dbms="SqlServer">[Login] IS NOT NULL AND [Login] &lt;&gt; N''</Predicate>
	<Predicate Dbms="PostgreSql">"Login" IS NOT NULL AND "Login" &lt;&gt; ''</Predicate>
</SchemeTable>