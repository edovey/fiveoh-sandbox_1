//
//  RootViewController.m
//  tmp
//
//  Created by Liz Dovey on 11-09-23.
//  Copyright 2011 875953 Alberta, Inc. All rights reserved.
//

#import "RootViewController.h"
#import "ChapterView.h"

@implementation RootViewController


- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    [self.navigationController setNavigationBarHidden:YES animated:YES];
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Relinquish ownership any cached data, images, etc that aren't in use.
}

- (void)viewDidUnload
{
    [super viewDidUnload];

    // Relinquish ownership of anything that can be recreated in viewDidLoad or on demand.
    // For example: self.myOutlet = nil;
}

- (void)dealloc
{
    [super dealloc];
}

#pragma mark - UI Actions

-(IBAction)openTouched:(id)sender 
{
    ChapterView *vwChapters = [[ChapterView alloc] initWithNibName:@"ChapterView" bundle:nil];
    [self.navigationController pushViewController:vwChapters animated:YES];
    [vwChapters release];
}

@end
