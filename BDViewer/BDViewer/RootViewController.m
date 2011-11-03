//
//  RootViewController.m
//  tmp
//
//  Created by Liz Dovey on 11-09-23.
//  Copyright 2011 875953 Alberta, Inc. All rights reserved.
//

#import "RootViewController.h"
#import "ChapterListView.h"
#import "RepositoryHandler.h"

@implementation RootViewController
@synthesize syncActivityIndictor;


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
    [self setSyncActivityIndictor:nil];
    [super viewDidUnload];

    // Relinquish ownership of anything that can be recreated in viewDidLoad or on demand.
    // For example: self.myOutlet = nil;
}

- (void)dealloc
{
    [syncActivityIndictor release];
    [super dealloc];
}

#pragma mark - UI Actions

-(IBAction)openTouched:(id)sender 
{
     ChapterListView *vwChapters = [[ChapterListView alloc] initWithNibName:@"ChapterListView" bundle:nil];
    [self.navigationController pushViewController:vwChapters animated:YES];
    [vwChapters release];
}

-(IBAction)loadTouched:(id)sender
{
    [syncActivityIndicator startAnimating];
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSObject *refreshDateInfo = [defaults  objectForKey:REPOSITORY_REFRESHDATE_KEY]; // Get the datetime of the last refresh
    NSDate *lastRefreshDate = (NSDate *)refreshDateInfo;
    [RepositoryHandler pullSince:lastRefreshDate];
    [syncActivityIndicator stopAnimating];
}

@end
