//
//  NSString+UUID.m
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-18.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "NSString+UUID.h"

@implementation NSString (UUID)

+ (NSString*) UUIDCreate
{
	CFUUIDRef newUUID = CFUUIDCreate(NULL);
	CFStringRef stringRef = CFUUIDCreateString(NULL, newUUID);
	CFRelease(newUUID);
    NSString *string = (NSString *)stringRef;
    NSString *uppercase = [string uppercaseString];
    [string release];
	return uppercase;
}

+ (NSString*) UUIDEmpty
{
	return @"00000000-0000-0000-0000-000000000000";
}

//+ (NSString *) UUIDCreate
//{
//	CFUUIDRef newUUID = CFUUIDCreate(NULL);
//	CFStringRef stringRef = CFUUIDCreateString(NULL, newUUID);
//	CFRelease(newUUID);
//    
//    NSString *string = (__bridge_transfer NSString *)stringRef;
//    
//    NSString *uppercase = [string uppercaseString];
//	return uppercase;
//}
//
//+ (NSString *) UUIDEmpty
//{
//	return @"00000000-0000-0000-0000-000000000000";
//}
//
@end
