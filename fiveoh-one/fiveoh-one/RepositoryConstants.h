/*
 * Copyright 2010-2011 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

#import <Foundation/Foundation.h>

#import <AWSiOSSDK/S3/AmazonS3Client.h>
#import <AWSiOSSDK/SimpleDB/AmazonSimpleDBClient.h>
#import <AWSiOSSDK/SQS/AmazonSQSClient.h>
#import <AWSiOSSDK/SNS/AmazonSNSClient.h>

//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// This sample App is for demonstration purposes only.
// It is not secure to embed your credentials into source code.
// Please read the following article for getting credentials
// to devices securely.
// http://aws.amazon.com/articles/Mobile/4611615499399490
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

// B&D project: This is a defined user with limited access
#define ACCESS_KEY                   @"AKIAJ6SRLQLH2ALT7ZBQ"
#define SECRET_KEY                   @"djtyS8sx5dKxifZ6oDT6gNgzp4HktsZYMnFlNPfp"

#define CREDENTIALS_ALERT_MESSAGE    @"Please update the Constants.h file with your credentials."

@interface RepositoryConstants:NSObject 
{
}

+(AmazonS3Client *)s3;
+(AmazonSimpleDBClient *)sdb;
+(AmazonSQSClient *)sqs;
+(AmazonSNSClient *)sns;

+(UIAlertView *)credentialsAlert;

@end
