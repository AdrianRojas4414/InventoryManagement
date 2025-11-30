using Reqnroll;
using Xunit; // Necesario para Assert.Equal

namespace ReqnrollIntegrationTests.StepDefinitions
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {
        // Variables privadas para almacenar los datos entre los pasos (Given/When/Then)
        private int _firstNumber;
        private int _secondNumber;
        private int _result;

        [Given("the first number is {int}")]
        public void GivenTheFirstNumberIs(int number)
        {
            // Guardamos el número que viene del archivo .feature
            _firstNumber = number;
        }

        [Given("the second number is {int}")]
        public void GivenTheSecondNumberIs(int number)
        {
            // Guardamos el segundo número
            _secondNumber = number;
        }

        [When("the two numbers are added")]
        public void WhenTheTwoNumbersAreAdded()
        {
            // Ejecutamos la lógica de la prueba (la suma)
            _result = _firstNumber + _secondNumber;
        }

        [Then("the result should be {int}")]
        public void ThenTheResultShouldBe(int expectedResult)
        {
            // Verificamos que el resultado real (_result) coincida con el esperado (expectedResult)
            // Usamos Assert de xUnit ya que tu proyecto lo tiene referenciado
            Assert.Equal(expectedResult, _result);
        }
    }
}