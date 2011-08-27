//
//  Utility.m
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-25.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "Utility.h"

@implementation Utility

+ (NSString*) nsNumberBoolToString:(NSNumber *)theValue
{
    return [theValue boolValue] ? @"true" : @"false";
}

@end
