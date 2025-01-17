namespace Shilony.PlainDataSource.Test
{
	using System;
	using System.Text;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class StringBuilderExtensionsTest
	{
		private const string DB_NULL_VALUE = "(dbnull)";

		private const string NULL_VALUE = "(null)";

		private StringBuilder _stringBuilder;

		// Use TestInitialize to run code before running each test 
		[TestInitialize]
		public void MyTestInitialize()
		{
			_stringBuilder = new StringBuilder();
		}

		[TestMethod]
		public void AppendParameter_DbNullValue_ReturnValueAsDbNullString()
		{
			object value = DBNull.Value;

			_stringBuilder.AppendParameter(value);

			Assert.AreEqual(DB_NULL_VALUE, _stringBuilder.ToString());
		}

		[TestMethod]
		public void AppendParameter_NullValue_ReturnValueAsNullString()
		{
			const object value = null;

			_stringBuilder.AppendParameter(value);

			Assert.AreEqual(NULL_VALUE, _stringBuilder.ToString());
		}

		[TestMethod]
		public void AppendParameter_ValidValue_ReturnTheProvidedValue()
		{
			const string value = "123456789";

			const string param_value = "=" + value;

			_stringBuilder.AppendParameter(value);

			Assert.AreEqual(param_value, _stringBuilder.ToString());
		}

		[TestMethod]
		public void AppendWithSeparator_NullValue_ReturnWithEmptyString()
		{
			const string value = null;

			_stringBuilder.AppendWithSeparator(value);

			Assert.AreEqual(string.Empty, _stringBuilder.ToString());
		}

		[TestMethod]
		public void AppendWithSeparator_ValidValue_ReturnValueWithSeparator()
		{
			const string value = "123456789";

			const string value_with_separator = ":" + value;

			_stringBuilder.AppendWithSeparator(value);

			Assert.AreEqual(value_with_separator, _stringBuilder.ToString());
		}
	}
}