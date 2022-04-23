
--------Aşağıdaki kod çağırın
SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

--------App confiğe bu satırları eklenyin
<dependentAssembly>
	<assemblyIdentity name="Microsoft.SqlServer.Types" publicKeyToken="89845dcd8080cc91" culture="neutral" />
	<bindingRedirect oldVersion="10.0.0.0" newVersion="14.0.0.0" />
</dependentAssembly>
