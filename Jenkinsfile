pipeline { 
    agent any 
    stages {
        stage('Build') { 
            steps { 
                sh 'cd idetector && dotnet build' 
            }
        }
        stage('Test'){
            steps {
                sh 'cd idetector/xUnitTest && dotnet test --logger:xunit'
                sh 'dotnet xunit-to-junit "detector/xUnitTest/TestResults/TestResults.xml" "detector/xUnitTest/TestResults/Testresults.junit.xml"'
                junit ' idetector/xUnitTest/TestResults/*.junit.xml' 
            }
        }
    }
}