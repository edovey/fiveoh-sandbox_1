//
//  ChapterView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "ChapterView.h"
#import "BDChapter.h"
#import "SectionView.h"
#import "DataController.h"

@implementation ChapterView
@synthesize dataTableView;
@synthesize chapterArray;

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
    
    [self.navigationController setNavigationBarHidden:NO animated:YES];
    self.title = @"Chapters";
}

-(void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];    
    self.chapterArray = [NSArray arrayWithArray:[BDChapter retrieveAll]];
}

- (void)viewDidUnload
{
    [self setDataTableView:nil];
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (void)dealloc {
    [dataTableView release];
    [chapterArray release];
    [super dealloc];
}

#pragma mark - TableView DataSource

- (int)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (int)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [self.chapterArray count];
}

#pragma mark - TableView Delegate

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [dataTableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
    }
    cell.textLabel.text = [[self.chapterArray objectAtIndex:indexPath.row] name];
    cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    BDChapter *chapter = [chapterArray objectAtIndex:indexPath.row];
    SectionView *vwSection = [[SectionView alloc] initWithParentId:[chapter uuid] withParentName: [chapter name]];
     [self.navigationController pushViewController:vwSection animated:YES];
     [vwSection release];
}



@end
