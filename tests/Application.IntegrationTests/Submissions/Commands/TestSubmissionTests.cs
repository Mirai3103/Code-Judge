using Code_Judge.Application.Problems.Queries.GetProblemBySlug;
using Code_Judge.Application.Submissions.TestSubmission;
using Code_Judge.Domain.Enums;
using NUnit.Framework;

namespace Code_Judge.Application.IntegrationTests.Submissions.Commands;

public class TestSubmissionTests
{
    [Test]
    public async Task ShouldPassTestSubmission()
    {
        // Arrange
        string cppCode = @"
#include <iostream>
#include <vector>
#include <unordered_map>

std::vector<int> twoSum(std::vector<int>& nums, int target) {
    std::unordered_map<int, int> numToIndex;
    std::vector<int> result;
    
    for (int i = 0; i < nums.size(); ++i) {
        int complement = target - nums[i];
        if (numToIndex.find(complement) != numToIndex.end()) {
            result.push_back(numToIndex[complement]);
            result.push_back(i);
            break;
        }
        numToIndex[nums[i]] = i;
    }
    
    return result;
}

int main() {
    int n;
    std::cin >> n;
    
    std::vector<int> nums(n);
    for (int i = 0; i < n; ++i) {
        std::cin >> nums[i];
    }
    
    int target;
    std::cin >> target;
    
    std::vector<int> result = twoSum(nums, target);
    
    if (result.size() == 2) {
        std::cout << result[0] << "" "" << result[1] << std::endl;
    } else {
        std::cout << ""No solution found."" << std::endl;
    }
    
    return 0;
}";

        var problem = await Testing.SendAsync(new GetProblemBySlugQuery("two-sum"));
        Assert.That(problem, Is.Not.Null);
        var request = new TestSubmissionCommand() { ProblemId =problem!.Id, Code = cppCode, Language = ProgramingLanguage.Cpp };
        var result =await Testing.SendAsync(request);

        // Act & Assert
        Assert.That(result, Is.Not.Null);
        Assert.Greater(result.Count(), 0);
        Assert.That(result.All(x => x.Status==SubmissionStatus.Accepted), Is.True);
    }
   
 
}