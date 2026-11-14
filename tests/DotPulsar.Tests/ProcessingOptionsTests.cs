/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar.Tests;

[Trait("Category", "Unit")]
public sealed class ProcessingOptionsTests
{
    [Fact]
    public void Constructor_GivenDefaults_ShouldNotCorrelateTraces()
    {
        //Act
        var options = new ProcessingOptions();

        //Assert
        options.TraceCorrelation.ShouldBe(TraceCorrelation.None);
        options.LinkTraces.ShouldBeFalse();
    }

    [Fact]
    public void LinkTraces_GivenTrue_ShouldSetTraceCorrelationToLink()
    {
        //Arrange
        var options = new ProcessingOptions();

        //Act
        options.LinkTraces = true;

        //Assert
        options.TraceCorrelation.ShouldBe(TraceCorrelation.Link);
        options.LinkTraces.ShouldBeTrue();
    }

    [Fact]
    public void LinkTraces_GivenFalse_ShouldSetTraceCorrelationToNone()
    {
        //Arrange
        var options = new ProcessingOptions { TraceCorrelation = TraceCorrelation.Parent };

        //Act
        options.LinkTraces = false;

        //Assert
        options.TraceCorrelation.ShouldBe(TraceCorrelation.None);
        options.LinkTraces.ShouldBeFalse();
    }

    [Fact]
    public void TraceCorrelation_GivenParent_ShouldNotReportLinkTraces()
    {
        //Arrange
        var options = new ProcessingOptions();

        //Act
        options.TraceCorrelation = TraceCorrelation.Parent;

        //Assert
        options.TraceCorrelation.ShouldBe(TraceCorrelation.Parent);
        options.LinkTraces.ShouldBeFalse();
    }

    [Fact]
    public void TraceCorrelation_GivenUndefinedValue_ShouldThrowArgumentOutOfRangeException()
    {
        //Arrange
        var options = new ProcessingOptions();

        //Act
        var exception = Record.Exception(() => options.TraceCorrelation = (TraceCorrelation) 42);

        //Assert
        exception.ShouldBeOfType<ArgumentOutOfRangeException>();
    }
}
